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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, TenSach = "Lập trình C#", TacGia = "Nguyễn Văn A", NhaSanXuat = "NXB BKHN", SoLuong = 10 },
                new Book { Id = 2, TenSach = "SQL Server", TacGia = "Trần Thị B", NhaSanXuat = "NXB Tin học", SoLuong = 8 },
                new Book { Id = 3, TenSach = "AI cơ bản", TacGia = "Lê Văn C", NhaSanXuat = "NXB Công nghệ", SoLuong = 12 }
            );
        }
    }
}
