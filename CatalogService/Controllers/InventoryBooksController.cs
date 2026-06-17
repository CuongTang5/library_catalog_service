using CatalogService.Data;
using CatalogService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatalogService.Dtos;

namespace CatalogService.Controllers
{
    [Route("api/inventory-books")]
    [ApiController]
    [Authorize]
    public class InventoryBooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryBooksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetInventoryBooks()
        {
            var inventoryBooks = await _context.InventoryBooks
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return Ok(inventoryBooks.Select(b => new
            {
                b.Id,
                b.TenSach,
                b.TacGia,
                b.NhaSanXuat,
                b.TheLoai,
                b.SoLuongTonKho,
                b.ImageUrl,
                b.MoTa,
                b.Isbn,
                namXuatBan = b.NamXuatBan,
                tomTat = b.TomTat,
                createdAt = b.CreatedAt
            }));
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateInventoryBook([FromBody] CreateInventoryBookDto? request)
        {
            if (request is null)
            {
                return BadRequest("Dữ liệu nhập kho không hợp lệ.");
            }

            var tenSach = request.TenSach?.Trim();
            if (string.IsNullOrWhiteSpace(tenSach))
            {
                return BadRequest("Tên sách không được để trống.");
            }

            if (request.SoLuongTonKho <= 0)
            {
                return BadRequest("Số lượng nhập kho phải lớn hơn 0.");
            }

            var inventoryBook = new InventoryBook
            {
                TenSach = tenSach,
                TacGia = request.TacGia?.Trim() ?? string.Empty,
                NhaSanXuat = request.NhaSanXuat?.Trim() ?? string.Empty,
                TheLoai = string.IsNullOrWhiteSpace(request.TheLoai) ? null : request.TheLoai?.Trim(),
                SoLuongTonKho = request.SoLuongTonKho,
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl?.Trim(),
                MoTa = string.IsNullOrWhiteSpace(request.MoTa) ? null : request.MoTa?.Trim(),
                Isbn = string.IsNullOrWhiteSpace(request.Isbn) ? null : request.Isbn?.Trim(),
                NamXuatBan = request.NamXuatBan,
                TomTat = string.IsNullOrWhiteSpace(request.TomTat) ? null : request.TomTat?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.InventoryBooks.Add(inventoryBook);

            var receipt = new InventoryImportReceipt
            {
                Code = $"PN-{DateTime.UtcNow:yyyyMMddHHmmss}",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "admin",
                Source = "Manual",
                Note = "Nhập kho thủ công bằng form",
                TotalItems = 1,
                TotalQuantity = inventoryBook.SoLuongTonKho
            };
            _context.InventoryImportReceipts.Add(receipt);

            var receiptItem = new InventoryImportReceiptItem
            {
                Receipt = receipt,
                InventoryBook = inventoryBook,
                TenSach = inventoryBook.TenSach,
                Quantity = inventoryBook.SoLuongTonKho,
                Note = "Nhập kho thủ công"
            };
            _context.InventoryImportReceiptItems.Add(receiptItem);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInventoryBooks), new { id = inventoryBook.Id }, new
            {
                inventoryBook.Id,
                inventoryBook.TenSach,
                inventoryBook.TacGia,
                inventoryBook.NhaSanXuat,
                inventoryBook.TheLoai,
                inventoryBook.SoLuongTonKho,
                inventoryBook.ImageUrl,
                inventoryBook.MoTa,
                inventoryBook.Isbn,
                namXuatBan = inventoryBook.NamXuatBan,
                tomTat = inventoryBook.TomTat,
                createdAt = inventoryBook.CreatedAt
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateInventoryBook(int id, [FromBody] UpdateInventoryBookDto dto)
        {
            var existingBook = await _context.InventoryBooks.FindAsync(id);
            if (existingBook is null)
            {
                return NotFound("Không tìm thấy sách trong kho nhập.");
            }

            // Cập nhật các trường mô tả
            existingBook.TenSach = dto.TenSach;
            existingBook.TacGia = dto.TacGia;
            existingBook.NhaSanXuat = dto.NhaSanXuat;
            existingBook.TheLoai = dto.TheLoai;
            existingBook.ImageUrl = dto.ImageUrl;
            existingBook.MoTa = dto.MoTa;
            existingBook.Isbn = dto.Isbn;
            existingBook.NamXuatBan = dto.NamXuatBan;
            existingBook.TomTat = dto.TomTat;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class AddToCatalogRequest
        {
            public int Quantity { get; set; }
        }

        [HttpPost("{id:int}/add-to-catalog")]
        public async Task<ActionResult<object>> AddInventoryBookToCatalog(int id, [FromBody] AddToCatalogRequest? request)
        {
            if (request is null || request.Quantity <= 0)
            {
                return BadRequest("Số lượng phải lớn hơn 0.");
            }

            var inventoryBook = await _context.InventoryBooks.FindAsync(id);
            if (inventoryBook is null)
            {
                return NotFound();
            }

            if (request.Quantity > inventoryBook.SoLuongTonKho)
            {
                return BadRequest("Số lượng không được lớn hơn tồn kho.");
            }

            var normalizedIsbn = inventoryBook.Isbn?.Trim();
            var normalizedTenSach = inventoryBook.TenSach.Trim();
            Book? existingBook = null;

            if (!string.IsNullOrWhiteSpace(normalizedIsbn))
            {
                existingBook = await _context.Books
                    .FirstOrDefaultAsync(b => b.Isbn != null && b.Isbn.Trim().ToLower() == normalizedIsbn.ToLower());
            }
            else
            {
                var normalizedTacGia = inventoryBook.TacGia.Trim();
                existingBook = await _context.Books
                    .FirstOrDefaultAsync(b => b.TenSach.Trim().ToLower() == normalizedTenSach.ToLower()
                                           && b.TacGia.Trim().ToLower() == normalizedTacGia.ToLower());
            }

            if (existingBook is null)
            {
                existingBook = new Book
                {
                    TenSach = inventoryBook.TenSach,
                    TacGia = inventoryBook.TacGia,
                    NhaSanXuat = inventoryBook.NhaSanXuat,
                    TheLoai = inventoryBook.TheLoai,
                    SoLuong = request.Quantity,
                    SoBanDaMuon = 0,
                    ImageUrl = inventoryBook.ImageUrl,
                    MoTa = inventoryBook.MoTa,
                    Isbn = inventoryBook.Isbn,
                    NamXuatBan = inventoryBook.NamXuatBan,
                    TomTat = inventoryBook.TomTat
                };
                _context.Books.Add(existingBook);
            }
            else
            {
                existingBook.SoLuong += request.Quantity;
            }

            inventoryBook.SoLuongTonKho -= request.Quantity;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                inventoryBook.Id,
                inventoryBook.SoLuongTonKho,
                bookId = existingBook.Id,
                existingBook.TenSach,
                transferredQuantity = request.Quantity
            });
        }

        public class BatchAddToCatalogItem
        {
            public int InventoryBookId { get; set; }
            public int Quantity { get; set; }
        }

        public class BatchAddToCatalogRequest
        {
            public List<int>? InventoryBookIds { get; set; }
            public int QuantityPerBook { get; set; }
            public List<BatchAddToCatalogItem>? Items { get; set; }
        }

        [HttpPost("batch-add-to-catalog")]
        public async Task<ActionResult<object>> BatchAddToCatalog([FromBody] BatchAddToCatalogRequest? request)
        {
            if (request is null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            var itemsToProcess = new List<BatchAddToCatalogItem>();
            if (request.Items != null && request.Items.Count > 0)
            {
                itemsToProcess = request.Items;
            }
            else if (request.InventoryBookIds != null && request.InventoryBookIds.Count > 0)
            {
                if (request.QuantityPerBook <= 0)
                {
                    return BadRequest("Số lượng phải lớn hơn 0.");
                }
                itemsToProcess = request.InventoryBookIds.Select(id => new BatchAddToCatalogItem
                {
                    InventoryBookId = id,
                    Quantity = request.QuantityPerBook
                }).ToList();
            }

            if (itemsToProcess.Count == 0)
            {
                return BadRequest("Danh sách sách cần thêm trống.");
            }

            int successCount = 0;
            int failedCount = 0;
            var failedIds = new List<int>();

            foreach (var item in itemsToProcess)
            {
                var id = item.InventoryBookId;
                var qty = item.Quantity;

                if (qty <= 0)
                {
                    failedCount++;
                    failedIds.Add(id);
                    continue;
                }

                try
                {
                    var inventoryBook = await _context.InventoryBooks.FindAsync(id);
                    if (inventoryBook is null)
                    {
                        failedCount++;
                        failedIds.Add(id);
                        continue;
                    }

                    if (qty > inventoryBook.SoLuongTonKho)
                    {
                        failedCount++;
                        failedIds.Add(id);
                        continue;
                    }

                    var normalizedIsbn = inventoryBook.Isbn?.Trim();
                    var normalizedTenSach = inventoryBook.TenSach.Trim();
                    Book? existingBook = null;

                    if (!string.IsNullOrWhiteSpace(normalizedIsbn))
                    {
                        existingBook = await _context.Books
                            .FirstOrDefaultAsync(b => b.Isbn != null && b.Isbn.Trim().ToLower() == normalizedIsbn.ToLower());
                    }
                    else
                    {
                        var normalizedTacGia = inventoryBook.TacGia.Trim();
                        existingBook = await _context.Books
                            .FirstOrDefaultAsync(b => b.TenSach.Trim().ToLower() == normalizedTenSach.ToLower()
                                                   && b.TacGia.Trim().ToLower() == normalizedTacGia.ToLower());
                    }

                    if (existingBook is null)
                    {
                        existingBook = new Book
                        {
                            TenSach = inventoryBook.TenSach,
                            TacGia = inventoryBook.TacGia,
                            NhaSanXuat = inventoryBook.NhaSanXuat,
                            TheLoai = inventoryBook.TheLoai,
                            SoLuong = qty,
                            SoBanDaMuon = 0,
                            ImageUrl = inventoryBook.ImageUrl,
                            MoTa = inventoryBook.MoTa,
                            Isbn = inventoryBook.Isbn,
                            NamXuatBan = inventoryBook.NamXuatBan,
                            TomTat = inventoryBook.TomTat
                        };
                        _context.Books.Add(existingBook);
                    }
                    else
                    {
                        existingBook.SoLuong += qty;
                    }

                    inventoryBook.SoLuongTonKho -= qty;
                    await _context.SaveChangesAsync();
                    successCount++;
                }
                catch (Exception)
                {
                    failedCount++;
                    failedIds.Add(id);
                }
            }

            return Ok(new
            {
                successCount,
                failedCount,
                failedIds
            });
        }

        public class ExcelImportItem
        {
            public string? TenSach { get; set; }
            public string? TacGia { get; set; }
            public string? NhaSanXuat { get; set; }
            public string? TheLoai { get; set; }
            public int SoLuong { get; set; }
            public string? Isbn { get; set; }
            public string? MoTa { get; set; }
            public string? ImageUrl { get; set; }
            public int? NamXuatBan { get; set; }
            public string? TomTat { get; set; }
        }

        [HttpPost("import-excel")]
        public async Task<ActionResult<object>> ImportExcel([FromBody] List<ExcelImportItem>? items)
        {
            if (items is null || items.Count == 0)
            {
                return BadRequest("Danh sách sách không hợp lệ.");
            }

            var receipt = new InventoryImportReceipt
            {
                Code = $"PN-EX-{DateTime.UtcNow:yyyyMMddHHmmss}",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "admin",
                Source = "Excel",
                Note = "Nhập kho bằng Excel",
                TotalItems = 0,
                TotalQuantity = 0
            };
            _context.InventoryImportReceipts.Add(receipt);

            int imported = 0;
            int skipped = 0;

            foreach (var item in items)
            {
                var tenSach = item?.TenSach?.Trim();
                if (string.IsNullOrWhiteSpace(tenSach))
                {
                    skipped++;
                    continue;
                }

                var soLuong = item.SoLuong > 0 ? item.SoLuong : 1;
                var normalizedIsbn = item.Isbn?.Trim();
                var normalizedTenSach = tenSach;
                InventoryBook? existing = null;

                if (!string.IsNullOrWhiteSpace(normalizedIsbn))
                {
                    existing = await _context.InventoryBooks
                        .FirstOrDefaultAsync(b => b.Isbn != null && b.Isbn.Trim().ToLower() == normalizedIsbn.ToLower());
                }

                if (existing is null)
                {
                    existing = await _context.InventoryBooks
                        .FirstOrDefaultAsync(b => b.TenSach.Trim().ToLower() == normalizedTenSach.ToLower());
                }

                if (existing is null)
                {
                    existing = new InventoryBook
                    {
                        TenSach = tenSach,
                        TacGia = string.IsNullOrWhiteSpace(item.TacGia) ? "Chưa rõ" : item.TacGia.Trim(),
                        NhaSanXuat = string.IsNullOrWhiteSpace(item.NhaSanXuat) ? "Chưa rõ" : item.NhaSanXuat.Trim(),
                        TheLoai = string.IsNullOrWhiteSpace(item.TheLoai) ? "Chưa phân loại" : item.TheLoai.Trim(),
                        SoLuongTonKho = soLuong,
                        ImageUrl = string.IsNullOrWhiteSpace(item.ImageUrl) ? null : item.ImageUrl.Trim(),
                        MoTa = string.IsNullOrWhiteSpace(item.MoTa) ? null : item.MoTa.Trim(),
                        Isbn = string.IsNullOrWhiteSpace(item.Isbn) ? null : item.Isbn.Trim(),
                        NamXuatBan = item.NamXuatBan,
                        TomTat = string.IsNullOrWhiteSpace(item.TomTat) ? null : item.TomTat.Trim(),
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.InventoryBooks.Add(existing);
                }
                else
                {
                    existing.SoLuongTonKho += soLuong;
                }

                var receiptItem = new InventoryImportReceiptItem
                {
                    Receipt = receipt,
                    InventoryBook = existing,
                    TenSach = existing.TenSach,
                    Quantity = soLuong,
                    Note = "Nhập Excel"
                };
                _context.InventoryImportReceiptItems.Add(receiptItem);

                receipt.TotalItems++;
                receipt.TotalQuantity += soLuong;
                imported++;
            }

            await _context.SaveChangesAsync();
            return Ok(new { imported = imported, skipped = skipped });
        }

        [HttpPost("sync-from-books")]
        public async Task<ActionResult<object>> SyncFromBooks()
        {
            var books = await _context.Books.ToListAsync();
            int importedCount = 0;
            int updatedCount = 0;

            foreach (var book in books)
            {
                var normalizedIsbn = book.Isbn?.Trim();
                var normalizedTenSach = book.TenSach.Trim();
                InventoryBook? existing = null;

                if (!string.IsNullOrWhiteSpace(normalizedIsbn))
                {
                    existing = await _context.InventoryBooks
                        .FirstOrDefaultAsync(b => b.Isbn != null && b.Isbn.Trim().ToLower() == normalizedIsbn.ToLower());
                }
                if (existing == null)
                {
                    existing = await _context.InventoryBooks
                        .FirstOrDefaultAsync(b => b.TenSach.Trim().ToLower() == normalizedTenSach.ToLower());
                }

                if (existing != null)
                {
                    existing.SoLuongTonKho += book.SoLuong;
                    updatedCount++;
                }
                else
                {
                    var newInventoryBook = new InventoryBook
                    {
                        TenSach = book.TenSach,
                        TacGia = book.TacGia,
                        NhaSanXuat = book.NhaSanXuat,
                        TheLoai = book.TheLoai,
                        SoLuongTonKho = book.SoLuong,
                        ImageUrl = book.ImageUrl,
                        MoTa = book.MoTa,
                        Isbn = book.Isbn,
                        NamXuatBan = book.NamXuatBan,
                        TomTat = book.TomTat,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.InventoryBooks.Add(newInventoryBook);
                    importedCount++;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { imported = importedCount, updated = updatedCount });
        }
    }
}
