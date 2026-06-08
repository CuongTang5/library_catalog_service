using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Truyện ngắn" },
                    { 2, "Tiểu thuyết" },
                    { 3, "Văn học Việt Nam" },
                    { 4, "Văn học nước ngoài" },
                    { 5, "Thiếu nhi" },
                    { 6, "Kỹ năng sống" },
                    { 7, "Công nghệ thông tin" },
                    { 8, "Lập trình" },
                    { 9, "Khoa học máy tính" },
                    { 10, "Trí tuệ nhân tạo" },
                    { 11, "Khoa học" },
                    { 12, "Toán học" },
                    { 13, "Vật lý" },
                    { 14, "Hóa học" },
                    { 15, "Sinh học" },
                    { 16, "Kinh tế" },
                    { 17, "Marketing" },
                    { 18, "Quản trị kinh doanh" },
                    { 19, "Tài chính" },
                    { 20, "Kế toán" },
                    { 21, "Luật" },
                    { 22, "Y học" },
                    { 23, "Giáo dục" },
                    { 24, "Giáo trình" },
                    { 25, "Ngoại ngữ" },
                    { 26, "Tiếng Anh" },
                    { 27, "Lịch sử" },
                    { 28, "Địa lý" },
                    { 29, "Chính trị" },
                    { 30, "Triết học" },
                    { 31, "Tâm lý học" },
                    { 32, "Nghệ thuật" },
                    { 33, "Âm nhạc" },
                    { 34, "Du lịch" },
                    { 35, "Ẩm thực" },
                    { 36, "Tôn giáo" },
                    { 37, "Truyện tranh" },
                    { 38, "Light Novel" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
