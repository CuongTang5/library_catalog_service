using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryBookColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NamXuatBan",
                table: "InventoryBooks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TomTat",
                table: "InventoryBooks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NamXuatBan",
                table: "InventoryBooks");

            migrationBuilder.DropColumn(
                name: "TomTat",
                table: "InventoryBooks");
        }
    }
}
