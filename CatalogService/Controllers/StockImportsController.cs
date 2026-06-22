using CatalogService.Data;
using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers
{
    [Route("api/stock-imports")]
    [Route("api/[controller]")]
    [ApiController]
    public class StockImportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockImportsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetReceipts()
        {
            var receipts = await _context.StockImportReceipts
                .Include(r => r.Items)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(receipts.Select(r => new
            {
                r.Id,
                r.Code,
                r.ImportDate,
                r.CreatedBy,
                r.Note,
                r.Status,
                r.CreatedAt,
                r.ApprovedAt,
                r.CancelledAt,
                totalQuantity = r.Items.Sum(i => i.Quantity),
                goodQuantity = r.Items.Where(i => i.Condition == "Good").Sum(i => i.Quantity),
                defectiveQuantity = r.Items.Where(i => i.Condition != "Good").Sum(i => i.Quantity),
                itemCount = r.Items.Count
            }));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetReceipt(int id)
        {
            var receipt = await _context.StockImportReceipts
                .Include(r => r.Items)
                .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt is null)
                return NotFound();

            return Ok(new
            {
                receipt.Id,
                receipt.Code,
                receipt.ImportDate,
                receipt.CreatedBy,
                receipt.Note,
                receipt.Status,
                receipt.CreatedAt,
                receipt.ApprovedAt,
                receipt.CancelledAt,
                items = receipt.Items.Select(i => new
                {
                    i.Id,
                    i.BookId,
                    bookName = i.Book?.TenSach ?? "Unknown",
                    i.Quantity,
                    i.Condition,
                    i.Note
                })
            });
        }

        public class CreateStockImportRequest
        {
            public string? CreatedBy { get; set; }
            public string? Note { get; set; }
            public List<StockImportItemRequest>? Items { get; set; }
        }

        public class StockImportItemRequest
        {
            public int BookId { get; set; }
            public int Quantity { get; set; }
            public string? Condition { get; set; }
            public string? Note { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateReceipt([FromBody] CreateStockImportRequest request)
        {
            if (request is null)
                return BadRequest("Invalid request.");

            if (string.IsNullOrWhiteSpace(request.CreatedBy))
                return BadRequest("CreatedBy is required.");

            if (request.Items is null || request.Items.Count == 0)
                return BadRequest("At least one item is required.");

            // Generate code: SI-YYYYMMDD-HHMM
            var now = DateTime.UtcNow;
            var code = $"SI-{now:yyyyMMdd-HHmm}";

            var receipt = new StockImportReceipt
            {
                Code = code,
                ImportDate = now,
                CreatedBy = request.CreatedBy.Trim(),
                Note = request.Note?.Trim(),
                Status = "Pending",
                CreatedAt = now,
                Items = new List<StockImportItem>()
            };

            foreach (var itemReq in request.Items)
            {
                var book = await _context.Books.FindAsync(itemReq.BookId);
                if (book is null)
                    return BadRequest($"Book with ID {itemReq.BookId} not found.");

                var condition = itemReq.Condition?.Trim() ?? "Good";
                if (!new[] { "Good", "Damaged", "Burned", "Lost" }.Contains(condition))
                    condition = "Good";

                receipt.Items.Add(new StockImportItem
                {
                    BookId = itemReq.BookId,
                    Quantity = Math.Max(1, itemReq.Quantity),
                    Condition = condition,
                    Note = itemReq.Note?.Trim()
                });
            }

            _context.StockImportReceipts.Add(receipt);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReceipt), new { id = receipt.Id }, new
            {
                receipt.Id,
                receipt.Code,
                receipt.Status
            });
        }

        [HttpPost("{id:int}/approve")]
        public async Task<ActionResult<object>> ApproveReceipt(int id)
        {
            var receipt = await _context.StockImportReceipts
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt is null)
                return NotFound();

            if (receipt.Status != "Pending")
                return BadRequest("Only Pending receipts can be approved.");

            // Process items
            foreach (var item in receipt.Items)
            {
                if (item.Condition == "Good")
                {
                    var book = await _context.Books.FindAsync(item.BookId);
                    if (book is not null)
                    {
                        book.SoLuong += item.Quantity;
                    }
                }
                // Damaged, Burned, Lost items are not added to inventory
            }

            receipt.Status = "Approved";
            receipt.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                receipt.Id,
                receipt.Code,
                receipt.Status,
                receipt.ApprovedAt
            });
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<ActionResult<object>> CancelReceipt(int id)
        {
            var receipt = await _context.StockImportReceipts
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt is null)
                return NotFound();

            if (receipt.Status != "Pending")
                return BadRequest("Only Pending receipts can be cancelled.");

            receipt.Status = "Cancelled";
            receipt.CancelledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                receipt.Id,
                receipt.Code,
                receipt.Status,
                receipt.CancelledAt
            });
        }
    }
}
