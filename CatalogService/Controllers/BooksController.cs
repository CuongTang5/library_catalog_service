using CatalogService.Data;
using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _context.Books.ToListAsync();
            return Ok(books);
        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<object>>> GetProducts()
        {
            var products = await _context.Books
                .Select(b => new
                {
                    ma = b.Id.ToString(),
                    tenSanPham = b.TenSach,
                    tacGia = b.TacGia,
                    nhaSanXuat = b.NhaSanXuat,
                    soLuong = b.SoLuong,
                    soBanDaMuon = b.SoBanDaMuon,
                    soBanConLai = b.SoBanDaMuon >= 0 ? b.SoLuong - b.SoBanDaMuon : b.SoLuong,
                    trangThai = b.SoBanDaMuon < b.SoLuong ? "Có thể mượn" : "Hết sách"
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook is null)
            {
                return NotFound();
            }

            existingBook.TenSach = book.TenSach;
            existingBook.TacGia = book.TacGia;
            existingBook.NhaSanXuat = book.NhaSanXuat;
            existingBook.SoLuong = book.SoLuong;
            existingBook.SoBanDaMuon = book.SoBanDaMuon;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class BookQuantityUpdateRequest
        {
            public int Quantity { get; set; }
        }

        [HttpPost("{id:int}/borrow")]
        public async Task<IActionResult> BorrowBook(int id, [FromBody] BookQuantityUpdateRequest request)
        {
            if (request is null || request.Quantity <= 0)
            {
                return BadRequest("Quantity must be a positive integer.");
            }

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook is null)
            {
                return NotFound();
            }

            if (existingBook.SoBanDaMuon + request.Quantity > existingBook.SoLuong)
            {
                return BadRequest("Cannot borrow more books than available.");
            }

            existingBook.SoBanDaMuon += request.Quantity;
            await _context.SaveChangesAsync();

            return Ok(existingBook);
        }

        [HttpPost("{id:int}/return")]
        public async Task<IActionResult> ReturnBook(int id, [FromBody] BookQuantityUpdateRequest request)
        {
            if (request is null || request.Quantity <= 0)
            {
                return BadRequest("Quantity must be a positive integer.");
            }

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook is null)
            {
                return NotFound();
            }

            if (existingBook.SoBanDaMuon - request.Quantity < 0)
            {
                return BadRequest("Return quantity cannot exceed borrowed quantity.");
            }

            existingBook.SoBanDaMuon -= request.Quantity;
            await _context.SaveChangesAsync();

            return Ok(existingBook);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
