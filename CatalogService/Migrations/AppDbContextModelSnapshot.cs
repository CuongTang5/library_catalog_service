using CatalogService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CatalogService.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("CatalogService.Models.Book", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                b.Property<string>("NhaSanXuat")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("SoLuong")
                    .HasColumnType("int");

                b.Property<string>("TacGia")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("TenSach")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("Books");

                b.HasData(
                    new
                    {
                        Id = 1,
                        NhaSanXuat = "NXB BKHN",
                        SoLuong = 10,
                        TacGia = "Nguyễn Văn A",
                        TenSach = "Lập trình C#"
                    },
                    new
                    {
                        Id = 2,
                        NhaSanXuat = "NXB Tin học",
                        SoLuong = 8,
                        TacGia = "Trần Thị B",
                        TenSach = "SQL Server"
                    },
                    new
                    {
                        Id = 3,
                        NhaSanXuat = "NXB Công nghệ",
                        SoLuong = 12,
                        TacGia = "Lê Văn C",
                        TenSach = "AI cơ bản"
                    });
            });
        }
    }
}
