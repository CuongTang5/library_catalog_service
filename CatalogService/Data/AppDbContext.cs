using CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Review> Reviews { get; set; }
        public DbSet<StockImportReceipt> StockImportReceipts { get; set; }
        public DbSet<StockImportItem> StockImportItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, TenSach = "Lập trình C#", TacGia = "Nguyễn Văn A", NhaSanXuat = "NXB BKHN", SoLuong = 10 },
                new Book { Id = 2, TenSach = "SQL Server", TacGia = "Trần Thị B", NhaSanXuat = "NXB Tin học", SoLuong = 8 },
                new Book { Id = 3, TenSach = "AI cơ bản", TacGia = "Lê Văn C", NhaSanXuat = "NXB Công nghệ", SoLuong = 12 }
            );

            // Category configuration: unique Name
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Seed default categories (Ids chosen sequentially)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Truyện ngắn" },
                new Category { Id = 2, Name = "Tiểu thuyết" },
                new Category { Id = 3, Name = "Văn học Việt Nam" },
                new Category { Id = 4, Name = "Văn học nước ngoài" },
                new Category { Id = 5, Name = "Thiếu nhi" },
                new Category { Id = 6, Name = "Kỹ năng sống" },
                new Category { Id = 7, Name = "Công nghệ thông tin" },
                new Category { Id = 8, Name = "Lập trình" },
                new Category { Id = 9, Name = "Khoa học máy tính" },
                new Category { Id = 10, Name = "Trí tuệ nhân tạo" },
                new Category { Id = 11, Name = "Khoa học" },
                new Category { Id = 12, Name = "Toán học" },
                new Category { Id = 13, Name = "Vật lý" },
                new Category { Id = 14, Name = "Hóa học" },
                new Category { Id = 15, Name = "Sinh học" },
                new Category { Id = 16, Name = "Kinh tế" },
                new Category { Id = 17, Name = "Marketing" },
                new Category { Id = 18, Name = "Quản trị kinh doanh" },
                new Category { Id = 19, Name = "Tài chính" },
                new Category { Id = 20, Name = "Kế toán" },
                new Category { Id = 21, Name = "Luật" },
                new Category { Id = 22, Name = "Y học" },
                new Category { Id = 23, Name = "Giáo dục" },
                new Category { Id = 24, Name = "Giáo trình" },
                new Category { Id = 25, Name = "Ngoại ngữ" },
                new Category { Id = 26, Name = "Tiếng Anh" },
                new Category { Id = 27, Name = "Lịch sử" },
                new Category { Id = 28, Name = "Địa lý" },
                new Category { Id = 29, Name = "Chính trị" },
                new Category { Id = 30, Name = "Triết học" },
                new Category { Id = 31, Name = "Tâm lý học" },
                new Category { Id = 32, Name = "Nghệ thuật" },
                new Category { Id = 33, Name = "Âm nhạc" },
                new Category { Id = 34, Name = "Du lịch" },
                new Category { Id = 35, Name = "Ẩm thực" },
                new Category { Id = 36, Name = "Tôn giáo" },
                new Category { Id = 37, Name = "Truyện tranh" },
                new Category { Id = 38, Name = "Light Novel" }
            );

            // StockImportReceipt configuration
            modelBuilder.Entity<StockImportReceipt>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.HasMany(e => e.Items)
                    .WithOne(i => i.Receipt)
                    .HasForeignKey(i => i.ReceiptId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // StockImportItem configuration
            modelBuilder.Entity<StockImportItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Condition).IsRequired().HasMaxLength(20);
                entity.HasOne(e => e.Book)
                    .WithMany()
                    .HasForeignKey(e => e.BookId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
