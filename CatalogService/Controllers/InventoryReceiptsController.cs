using CatalogService.Data;
using CatalogService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Controllers
{
    [Route("api/inventory-receipts")]
    [ApiController]
    [Authorize]
    public class InventoryReceiptsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryReceiptsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetReceipts()
        {
            var receipts = await _context.InventoryImportReceipts
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.Code,
                    r.CreatedAt,
                    r.CreatedBy,
                    r.Source,
                    r.Note,
                    r.TotalItems,
                    r.TotalQuantity
                })
                .ToListAsync();

            return Ok(receipts);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetReceipt(int id)
        {
            var receipt = await _context.InventoryImportReceipts
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                receipt.Id,
                receipt.Code,
                receipt.CreatedAt,
                receipt.CreatedBy,
                receipt.Source,
                receipt.Note,
                receipt.TotalItems,
                receipt.TotalQuantity,
                items = receipt.Items.Select(i => new
                {
                    i.Id,
                    i.InventoryBookId,
                    i.TenSach,
                    i.Quantity,
                    i.Note
                })
            });
        }
    }
}
