using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSach = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TacGia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NhaSanXuat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "NhaSanXuat", "SoLuong", "TacGia", "TenSach" },
                values: new object[] { 1, "NXB BKHN", 10, "Nguyễn Văn A", "Lập trình C#" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "NhaSanXuat", "SoLuong", "TacGia", "TenSach" },
                values: new object[] { 2, "NXB Tin học", 8, "Trần Thị B", "SQL Server" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "NhaSanXuat", "SoLuong", "TacGia", "TenSach" },
                values: new object[] { 3, "NXB Công nghệ", 12, "Lê Văn C", "AI cơ bản" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
