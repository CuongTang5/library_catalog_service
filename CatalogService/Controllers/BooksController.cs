using CatalogService.Data;
using CatalogService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using CatalogService.Dtos;
using System.Linq;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private static readonly Dictionary<string, string> Descriptions = new()
        {
            { "Dế Mèn Phiêu Lưu Ký", "Câu chuyện tuổi thơ hài hước và sâu sắc về những chuyến phiêu lưu của Dế Mèn, khám phá tình bạn và dũng cảm." },
            { "Tắt Đèn", "Tác phẩm hiện thực phê phán phản ánh nỗi cơ cực của người nông dân và sự bế tắc xã hội." },
            { "Lão Hạc", "Truyện ngắn cảm động về tình cha con, nhân phẩm và nỗi đau trong cuộc sống nông thôn Việt Nam." },
            { "Đời Thừa", "Một lát cắt cuộc đời giàu cảm xúc, thể hiện nỗi cô đơn và thất vọng của con người trước số phận." },
            { "Sống Mòn", "Khắc họa cuộc sống bế tắc của tầng lớp lao động trong bối cảnh xã hội thay đổi." },
            { "Vợ Nhặt", "Tác phẩm miêu tả hoàn cảnh khó khăn nhưng ấm áp tình người giữa thời đói khát." },
            { "Vợ Chồng A Phủ", "Câu chuyện về sự áp bức và khát vọng tự do của người dân miền núi." },
            { "Rừng Xà Nu", "Tác phẩm về kháng chiến, tinh thần đoàn kết và hy sinh của đồng bào Tây Nguyên." },
            { "Chiếc Lược Ngà", "Truyện cảm động về tình cha con bị chia cắt bởi chiến tranh và nỗi nhớ da diết." },
            { "Lặng Lẽ Sa Pa", "Câu chuyện nhẹ nhàng về những con người âm thầm cống hiến nơi miền núi." },
            { "Những Ngôi Sao Xa Xôi", "Tập truyện ngắn phản ánh tuổi trẻ, ước mơ và trách nhiệm thời chiến." },
            { "Bước Đường Cùng", "Tác phẩm hiện thực xã hội khai thác số phận con người trong khủng hoảng và bi kịch." },
            { "Giông Tố", "Tiểu thuyết tâm lý xã hội về mâu thuẫn gia đình và áp lực xã hội ở nông thôn." },
            { "Truyện Kiều", "Tác phẩm cổ điển nổi tiếng của Nguyễn Du, khắc họa bi kịch, tình yêu và số phận con người." },
            { "Lục Vân Tiên", "Truyện thơ hào hiệp, ca ngợi nghĩa khí, tình người và lý tưởng nhân nghĩa truyền thống." },
            { "Cho Tôi Xin Một Vé Đi Tuổi Thơ", "Tập truyện/tuổi thơ đầy hoài niệm về những kỷ niệm và bài học thời trẻ con." },
            { "Kính Vạn Hoa", "Bộ truyện hài hước, nhiều màu sắc về tuổi thơ với những tình huống dở khóc dở cười." },
            { "Mắt Biếc", "Tiểu thuyết tình cảm giàu cảm xúc về mối tình đơn phương và ký ức thời trẻ." },
            { "Cô Gái Đến Từ Hôm Qua", "Câu chuyện nhẹ nhàng nhưng sâu lắng về tình yêu tuổi học trò và những lựa chọn trưởng thành." },
            { "Tôi Thấy Hoa Vàng Trên Cỏ Xanh", "Tác phẩm giàu hoài niệm, gợi nhớ tuổi thơ ở làng quê, tình bạn và những khám phá nhỏ bé." },
            { "Ngồi Khóc Trên Cây", "Truyện ngắn/phù hợp về những cảm xúc bi thương và sự trưởng thành nội tâm." },
            { "Làm Bạn Với Bầu Trời", "Câu chuyện truyền cảm hứng về ước mơ, hy vọng và sự kết nối với thiên nhiên." },
            { "Con Chim Xanh Biếc Bay Về", "Tiểu thuyết nhẹ nhàng về ký ức, tình yêu và hành trình tìm lại bản thân." },
            { "Tuổi Thơ Dữ Dội", "Tập hồi ký tuổi thơ trong bối cảnh chiến tranh, đầy dũng khí và mất mát." },
            { "Đất Rừng Phương Nam", "Tiểu thuyết phiêu lưu pha lẫn tình cảm, miêu tả vùng đất phương Nam và con người nơi đây." },
            { "Quê Nội", "Tác phẩm tình cảm gia đình, hướng về cội nguồn và tình quê sâu nặng." },
            { "Harry Potter và Hòn Đá Phù Thủy", "Khởi đầu loạt truyện phiêu lưu kỳ ảo về cậu bé phù thủy và thế giới phép thuật." },
            { "Harry Potter và Phòng Chứa Bí Mật", "Tập tiếp theo khám phá những bí ẩn trong trường Hogwarts và thử thách mới." },
            { "Harry Potter và Tên Tù Nhân Ngục Azkaban", "Tập với sắc thái tối hơn, hé lộ quá khứ và biến cố của các nhân vật." },
            { "Harry Potter và Chiếc Cốc Lửa", "Tập lớn lên với những thử thách quốc tế, tình bạn và cạnh tranh." },
            { "Nhà Giả Kim", "Hành trình tìm kiếm kho báu và ý nghĩa cuộc đời, đầy triết lý và cảm hứng." },
            { "Đắc Nhân Tâm", "Hướng dẫn nghệ thuật giao tiếp và lãnh đạo để tạo ảnh hưởng tích cực." },
            { "Không Gia Đình", "Tiểu thuyết cảm động về hành trình và tình người của những kẻ lang thang." },
            { "Bố Già", "Tác phẩm sâu sắc về quyền lực, gia đình và trách nhiệm trong xã hội." },
            { "Rừng Na Uy", "Tiểu thuyết tình cảm tâm lý về cô đơn, tình yêu và bi kịch tuổi trẻ." },
            { "Kafka Bên Bờ Biển", "Tiểu thuyết hiện đại pha trộn yếu tố huyền ảo, tâm lý và triết lý sâu sắc." },
            { "1984", "Tiểu thuyết dystopia về chế độ toàn trị, giám sát và mất tự do cá nhân." },
            { "Animal Farm", "Truyện ngụ ngôn chính trị phê phán chuyên chế thông qua hình ảnh trang trại động vật." },
            { "The Great Gatsby", "Tiểu thuyết Mỹ về giấc mơ, tình yêu và sự tan vỡ trong thập niên 1920." },
            { "To Kill a Mockingbird", "Tác phẩm về công lý, định kiến và lòng nhân ái ở miền Nam nước Mỹ." },
            { "The Lord of the Rings", "Sử thi giả tưởng về cuộc chiến giữa thiện và ác, tình bạn và hy sinh." },
            { "The Hobbit", "Tiền truyện phiêu lưu dẫn dắt tới thế giới Trung Địa và những chuyến đi kỳ thú." },
            { "A Game of Thrones", "Tiểu thuyết fantasy chính trị, mưu mô và cuộc tranh đoạt ngai vàng." },
            { "Atomic Habits", "Hướng dẫn xây dựng thói quen nhỏ để đạt kết quả lớn trong cuộc sống và công việc." },
            { "Rich Dad Poor Dad", "Cuốn sách về tư duy tài chính và sự khác biệt giữa các cách tiếp cận tiền bạc." },
            { "The Psychology of Money", "Phân tích quan điểm tâm lý về tiền, đầu tư và ra quyết định tài chính." },
            { "Deep Work", "Chiến lược tập trung sâu để đạt năng suất cao trong công việc trí tuệ." },
            { "Clean Code", "Hướng dẫn viết mã sạch, dễ đọc và bảo trì để cải thiện chất lượng phần mềm." },
            { "Design Patterns", "Tập hợp các mẫu thiết kế phần mềm kinh điển để giải quyết các vấn đề lập trình phổ biến." },
            { "The Pragmatic Programmer", "Lời khuyên thực tế và nguyên tắc để trở thành lập trình viên hiệu quả." },
            { "Introduction to Algorithms", "Tài liệu tham khảo toàn diện về thuật toán và cấu trúc dữ liệu." },
            { "Artificial Intelligence: A Modern Approach", "Giáo trình toàn diện về trí tuệ nhân tạo, lý thuyết và ứng dụng." },
            { "Refactoring", "Hướng dẫn kỹ thuật tái cấu trúc mã để cải thiện thiết kế mà không thay đổi hành vi." },
            { "Code Complete", "Bộ sách tham khảo về thực hành tốt trong xây dựng phần mềm và kỹ thuật lập trình." },
            { "Computer Networks", "Tổng quan về nguyên lý và giao thức mạng máy tính." },
            { "Operating System Concepts", "Giới thiệu các khái niệm cơ bản về hệ điều hành và thiết kế của chúng." },
            { "VueJS", "Tài liệu/ứng dụng cơ bản về phát triển giao diện với Vue.js." },
            { "Dotnet", "Tổng quan về nền tảng .NET và cách xây dựng ứng dụng trên đó." }
        };

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        private static object MapToBookResponse(Book b)
        {
            Descriptions.TryGetValue(b.TenSach ?? string.Empty, out var desc);
            var mota = !string.IsNullOrWhiteSpace(b.MoTa)
                ? b.MoTa
                : !string.IsNullOrWhiteSpace(desc)
                    ? desc
                    : $"Tác phẩm {b.TenSach} của tác giả {b.TacGia}.";
            var imageUrl = !string.IsNullOrWhiteSpace(b.ImageUrl)
                ? b.ImageUrl
                : $"https://picsum.photos/seed/book-{b.Id}/300/450";
            return new
            {
                b.Id,
                b.TenSach,
                b.TacGia,
                b.NhaSanXuat,
                b.SoLuong,
                b.SoBanDaMuon,
                b.SoBanConLai,
                b.TrangThai,
                imageUrl,
                moTa = mota,
                isbn = b.Isbn,
                theLoai = b.TheLoai,
                namXuatBan = b.NamXuatBan,
                tomTat = b.TomTat,
                danhGiaTrungBinh = b.DanhGiaTrungBinh,
                soLuotDanhGia = b.SoLuotDanhGia
            };
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> SearchBooks([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Ok(Enumerable.Empty<object>());
            }

            var books = await _context.Books
                .Where(b => EF.Functions.Like(b.TenSach, $"%{q}%") || EF.Functions.Like(b.TacGia, $"%{q}%"))
                .ToListAsync();

            return Ok(books.Select(MapToBookResponse));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetBooks()
        {
            var books = await _context.Books.ToListAsync();
            var bookIds = books.Select(b => b.Id).ToList();
            var allReviews = await _context.Reviews
                .Where(r => bookIds.Contains(r.BookId))
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            var reviewsByBook = allReviews.GroupBy(r => r.BookId)
                .ToDictionary(g => g.Key, g => g.Take(3).ToList());

            return Ok(books.Select(b =>
            {
                Descriptions.TryGetValue(b.TenSach ?? string.Empty, out var desc);
                var mota = !string.IsNullOrWhiteSpace(b.MoTa) ? b.MoTa
                    : !string.IsNullOrWhiteSpace(desc) ? desc
                    : $"Tác phẩm {b.TenSach} của tác giả {b.TacGia}.";
                var imageUrl = !string.IsNullOrWhiteSpace(b.ImageUrl)
                    ? b.ImageUrl : $"https://picsum.photos/seed/book-{b.Id}/300/450";
                reviewsByBook.TryGetValue(b.Id, out var latest);
                return (object)new
                {
                    b.Id, b.TenSach, b.TacGia, b.NhaSanXuat,
                    b.SoLuong, b.SoBanDaMuon, b.SoBanConLai, b.TrangThai,
                    imageUrl, moTa = mota, isbn = b.Isbn, theLoai = b.TheLoai,
                    namXuatBan = b.NamXuatBan, tomTat = b.TomTat,
                    danhGiaTrungBinh = b.DanhGiaTrungBinh,
                    soLuotDanhGia = b.SoLuotDanhGia,
                    latestReviews = (latest ?? new()).Select(MapReview)
                };
            }));
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
                    trangThai = b.SoBanDaMuon < b.SoLuong ? "Có thể mượn" : "Hết sách",
                    theLoai = b.TheLoai
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null) return NotFound();

            var latest = await _context.Reviews
                .Where(r => r.BookId == id)
                .OrderByDescending(r => r.CreatedAt)
                .Take(3)
                .ToListAsync();

            Descriptions.TryGetValue(book.TenSach ?? string.Empty, out var desc);
            var mota = !string.IsNullOrWhiteSpace(book.MoTa) ? book.MoTa
                : !string.IsNullOrWhiteSpace(desc) ? desc
                : $"Tác phẩm {book.TenSach} của tác giả {book.TacGia}.";
            var imageUrl = !string.IsNullOrWhiteSpace(book.ImageUrl)
                ? book.ImageUrl : $"https://picsum.photos/seed/book-{book.Id}/300/450";

            return Ok(new
            {
                book.Id, book.TenSach, book.TacGia, book.NhaSanXuat,
                book.SoLuong, book.SoBanDaMuon, book.SoBanConLai, book.TrangThai,
                imageUrl, moTa = mota, isbn = book.Isbn, theLoai = book.TheLoai,
                namXuatBan = book.NamXuatBan, tomTat = book.TomTat,
                danhGiaTrungBinh = book.DanhGiaTrungBinh,
                soLuotDanhGia = book.SoLuotDanhGia,
                latestReviews = latest.Select(MapReview)
            });
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(CreateBookDto dto)
        {
            var book = new Book
            {
                TenSach = dto.TenSach,
                TacGia = dto.TacGia,
                NhaSanXuat = dto.NhaSanXuat,
                SoLuong = dto.SoLuong,
                ImageUrl = dto.ImageUrl,
                MoTa = dto.MoTa,
                Isbn = dto.Isbn,
                TheLoai = dto.TheLoai,
                NamXuatBan = dto.NamXuatBan,
                TomTat = dto.TomTat
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        public class BookImportRequest
        {
            public string? TenSach { get; set; }
            public string? TacGia { get; set; }
            public string? NhaSanXuat { get; set; }
            public string? TheLoai { get; set; }
            public int SoLuong { get; set; }
            public int SoBanDaMuon { get; set; }
            public string? Isbn { get; set; }
            public string? MoTa { get; set; }
            public string? ImageUrl { get; set; }
            public int? NamXuatBan { get; set; }
            public string? TomTat { get; set; }
        }

        [HttpPost("import")]
        public async Task<ActionResult<object>> ImportBooks([FromBody] List<BookImportRequest>? items)
        {
            if (items is null)
            {
                return BadRequest("Danh sách sách không hợp lệ.");
            }

            var imported = 0;
            var skipped = 0;
            var booksToAdd = new List<Book>();

            foreach (var item in items)
            {
                var tenSach = item?.TenSach?.Trim();
                if (string.IsNullOrWhiteSpace(tenSach))
                {
                    skipped++;
                    continue;
                }

                var tacGia = string.IsNullOrWhiteSpace(item?.TacGia) ? "Chưa rõ" : item!.TacGia!.Trim();
                var nhaSanXuat = string.IsNullOrWhiteSpace(item?.NhaSanXuat) ? "Chưa rõ" : item!.NhaSanXuat!.Trim();
                var theLoai = string.IsNullOrWhiteSpace(item?.TheLoai) ? "Chưa phân loại" : item!.TheLoai!.Trim();
                var soLuong = item?.SoLuong > 0 ? item.SoLuong : 1;
                var soBanDaMuon = item?.SoBanDaMuon >= 0 ? item.SoBanDaMuon : 0;

                if (soBanDaMuon > soLuong)
                {
                    soBanDaMuon = 0;
                }

                booksToAdd.Add(new Book
                {
                    TenSach = tenSach,
                    TacGia = tacGia,
                    NhaSanXuat = nhaSanXuat,
                    TheLoai = theLoai,
                    SoLuong = soLuong,
                    SoBanDaMuon = soBanDaMuon,
                    Isbn = item?.Isbn?.Trim(),
                    MoTa = item?.MoTa?.Trim(),
                    ImageUrl = item?.ImageUrl?.Trim(),
                    NamXuatBan = item?.NamXuatBan,
                    TomTat = item?.TomTat?.Trim()
                });

                imported++;
            }

            if (booksToAdd.Count > 0)
            {
                _context.Books.AddRange(booksToAdd);
                await _context.SaveChangesAsync();
            }

            return Ok(new { imported, skipped });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto dto)
        {
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook is null)
            {
                return NotFound();
            }

            // Giữ nguyên SoBanDaMuon hiện có
            var currentBorrowed = existingBook.SoBanDaMuon;
            if (dto.SoLuong < currentBorrowed)
            {
                return BadRequest("Không thể giảm số lượng thấp hơn số bản đang được mượn.");
            }

            int diff = dto.SoLuong - existingBook.SoLuong;
            if (diff != 0)
            {
                InventoryBook? inventoryBook = null;
                var matchIsbn = !string.IsNullOrWhiteSpace(existingBook.Isbn) ? existingBook.Isbn.Trim() : dto.Isbn?.Trim();
                var matchTenSach = !string.IsNullOrWhiteSpace(existingBook.TenSach) ? existingBook.TenSach.Trim() : dto.TenSach?.Trim();

                if (!string.IsNullOrWhiteSpace(matchIsbn))
                {
                    inventoryBook = await _context.InventoryBooks
                        .FirstOrDefaultAsync(b => b.Isbn != null && b.Isbn.Trim().ToLower() == matchIsbn.ToLower());
                }
                if (inventoryBook == null && !string.IsNullOrWhiteSpace(matchTenSach))
                {
                    inventoryBook = await _context.InventoryBooks
                        .FirstOrDefaultAsync(b => b.TenSach.Trim().ToLower() == matchTenSach.ToLower());
                }

                if (inventoryBook == null)
                {
                    return BadRequest("Không tìm thấy sách tương ứng trong kho nhập.");
                }

                if (diff > 0)
                {
                    if (inventoryBook.SoLuongTonKho < diff)
                    {
                        return BadRequest("Số lượng vượt quá tồn kho");
                    }
                    inventoryBook.SoLuongTonKho -= diff;
                }
                else // diff < 0
                {
                    inventoryBook.SoLuongTonKho += Math.Abs(diff);
                }
            }

            // Chỉ cho sửa TenSach, TacGia, NhaSanXuat, TheLoai, ImageUrl, MoTa, Isbn, SoLuong, NamXuatBan, TomTat
            existingBook.TenSach = dto.TenSach;
            existingBook.TacGia = dto.TacGia;
            existingBook.NhaSanXuat = dto.NhaSanXuat;
            existingBook.TheLoai = dto.TheLoai;
            existingBook.ImageUrl = dto.ImageUrl;
            existingBook.MoTa = dto.MoTa;
            existingBook.Isbn = dto.Isbn;
            existingBook.SoLuong = dto.SoLuong;
            existingBook.NamXuatBan = dto.NamXuatBan;
            existingBook.TomTat = dto.TomTat;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class BookRatingRequest
        {
            public int Rating { get; set; }
        }

        [HttpPost("{id:int}/rating")]
        public async Task<ActionResult<object>> RateBook(int id, [FromBody] BookRatingRequest request)
        {
            if (request is null || request.Rating < 1 || request.Rating > 5)
            {
                return BadRequest("Rating must be an integer between 1 and 5.");
            }

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook is null)
            {
                return NotFound();
            }

            var oldAverage = existingBook.DanhGiaTrungBinh;
            var oldCount = existingBook.SoLuotDanhGia;
            var newCount = oldCount + 1;
            var newAverage = ((oldAverage * oldCount) + request.Rating) / newCount;

            existingBook.DanhGiaTrungBinh = newAverage;
            existingBook.SoLuotDanhGia = newCount;

            await _context.SaveChangesAsync();

            return Ok(MapToBookResponse(existingBook));
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

        public class ReviewRequest
        {
            public string? UserId { get; set; }
            public string? CardNumber { get; set; }
            public string? Username { get; set; }
            public string? FullName { get; set; }
            public int Rating { get; set; }
            public string? Comment { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        private static object MapReview(Models.Review r) => new
        {
            r.Id,
            r.BookId,
            r.UserId,
            r.CardNumber,
            username = r.Username,
            fullName = r.FullName,
            r.Rating,
            r.Comment,
            r.CreatedAt
        };

        [HttpGet("{id:int}/reviews")]
        public async Task<ActionResult<IEnumerable<object>>> GetReviews(int id)
        {
            if (!await _context.Books.AnyAsync(b => b.Id == id))
                return NotFound();

            var reviews = await _context.Reviews
                .Where(r => r.BookId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(reviews.Select(MapReview));
        }

        [HttpPost("{id:int}/reviews")]
        public async Task<ActionResult<object>> AddReview(int id, [FromBody] ReviewRequest request)
        {
            if (request.Rating < 0 || request.Rating > 5)
                return BadRequest("Rating must be between 0 and 5.");

            var book = await _context.Books.FindAsync(id);
            if (book is null) return NotFound();

            _context.Reviews.Add(new Models.Review
            {
                BookId = id,
                UserId = request.UserId,
                CardNumber = request.CardNumber,
                Username = request.Username,
                FullName = request.FullName,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = request.CreatedAt == default ? DateTime.UtcNow : request.CreatedAt
            });

            var newCount = book.SoLuotDanhGia + 1;
            book.DanhGiaTrungBinh = ((book.DanhGiaTrungBinh * book.SoLuotDanhGia) + request.Rating) / newCount;
            book.SoLuotDanhGia = newCount;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Review saved",
                bookId = id,
                averageRating = book.DanhGiaTrungBinh,
                reviewCount = book.SoLuotDanhGia
            });
        }

        public class WriteOffRequest
        {
            public int Quantity { get; set; }
            public string? Reason { get; set; }
            public string? Note { get; set; }
        }

        [HttpPost("{id:int}/write-off")]
        public async Task<ActionResult<object>> WriteOffBook(int id, [FromBody] WriteOffRequest request)
        {
            if (request.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0.");

            var book = await _context.Books.FindAsync(id);
            if (book is null) return NotFound();

            if (request.Quantity > book.SoLuong)
                return BadRequest("Quantity exceeds total book count.");

            book.SoLuong -= request.Quantity;
            if (book.SoBanDaMuon > 0)
                book.SoBanDaMuon = Math.Max(0, book.SoBanDaMuon - request.Quantity);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Book written off",
                bookId = id,
                quantity = request.Quantity,
                reason = request.Reason,
                totalQuantity = book.SoLuong,
                availableQuantity = book.SoBanConLai
            });
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
