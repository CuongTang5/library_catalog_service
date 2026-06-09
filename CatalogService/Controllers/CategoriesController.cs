using CatalogService.Data;
using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _db.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            return Ok(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            var name = (dto?.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Tên thể loại không được rỗng");
            if (string.Equals(name, "Khác", StringComparison.OrdinalIgnoreCase)) return BadRequest("Tên không hợp lệ");

            var existing = await _db.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
            if (existing != null) return Ok(existing);

            var cat = new Category { Name = name };
            _db.Categories.Add(cat);
            try
            {
                await _db.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = cat.Id }, cat);
            }
            catch (DbUpdateException)
            {
                // likely due to unique constraint race — return existing
                var ex = await _db.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
                if (ex != null) return Ok(ex);
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryCreateDto dto)
        {
            var name = (dto?.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Tên thể loại không được rỗng");
            if (string.Equals(name, "Khác", StringComparison.OrdinalIgnoreCase)) return BadRequest("Tên không hợp lệ");

            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();

            var existing = await _db.Categories.FirstOrDefaultAsync(c => c.Id != id && c.Name.ToLower() == name.ToLower());
            if (existing != null) return BadRequest("Tên thể loại đã tồn tại");

            cat.Name = name;
            await _db.SaveChangesAsync();
            return Ok(cat);
        }

        [HttpGet("{id:int}/usage")]
        public async Task<IActionResult> GetUsage(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();

            var nameToCheck = cat.Name.Trim();
            var books = await _db.Books.Where(b => b.TheLoai != null).ToListAsync();
            var usedBy = books
                .Where(b => (b.TheLoai ?? string.Empty)
                    .Split(',')
                    .Select(x => x.Trim())
                    .Any(x => string.Equals(x, nameToCheck, StringComparison.OrdinalIgnoreCase)))
                .Select(b => b.TenSach)
                .ToList();

            return Ok(new { isUsed = usedBy.Count > 0, bookNames = usedBy });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool force = false)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();

            var nameToCheck = cat.Name.Trim();
            var books = await _db.Books.Where(b => b.TheLoai != null).ToListAsync();
            var usedBooks = books.Where(b =>
                (b.TheLoai ?? string.Empty)
                    .Split(',')
                    .Select(x => x.Trim())
                    .Any(x => string.Equals(x, nameToCheck, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (usedBooks.Count > 0 && !force)
            {
                return Conflict(new { message = "Thể loại đang được sử dụng", bookNames = usedBooks.Select(b => b.TenSach) });
            }

            if (usedBooks.Count > 0 && force)
            {
                foreach (var book in usedBooks)
                {
                    var parts = (book.TheLoai ?? string.Empty)
                        .Split(',')
                        .Select(x => x.Trim())
                        .Where(x => !string.Equals(x, nameToCheck, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    book.TheLoai = parts.Count > 0 ? string.Join(", ", parts) : "Chưa phân loại";
                }
            }

            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

    public class CategoryCreateDto
    {
        public string? Name { get; set; }
    }
}
