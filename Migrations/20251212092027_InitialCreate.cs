using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyBlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdminUserName = table.Column<string>(type: "text", nullable: false),
                    AdminPasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductName = table.Column<string>(type: "text", nullable: false),
                    ProductPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductStock = table.Column<int>(type: "integer", nullable: false),
                    ImageFile = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "AdminPasswordHash", "AdminUserName" },
                values: new object[,]
                {
                    { 1, "Tech1", "TechAdmin1" },
                    { 2, "Tech2", "TechAdmin2" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "ImageFile", "ProductName", "ProductPrice", "ProductStock" },
                values: new object[,]
                {
                    { 1, "", "Asus Vivobook X1605VA-OLED-SH2182W Laptop 16 Core i7 13620H/16 GB/1 TB/UHD Graphics/Windows 11 Home", 699m, 10 },
                    { 2, "", "Dell Alienware Aurora 16 Laptop 16 WQXGA Core 7 240H/32 GB/1 TB/RTX 5060 8 GB/Windows 11 Home", 1299m, 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
