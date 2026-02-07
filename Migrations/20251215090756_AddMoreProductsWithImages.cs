using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyBlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreProductsWithImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "ImageFile", "ProductName", "ProductPrice", "ProductStock" },
                values: new object[,]
                {
                    { 3, "/images/products/mac1.png", "Apple MacBook Air 13 M4 8-Core/16GB/256GB/10-Core GPU Midnight Laptop", 1149m, 20 },
                    { 4, "/images/products/mac2.png", "Apple MacBook Pro 16 M4 Max 16-Core/48GB/1TB/40-Core GPU Silver Laptop", 4839m, 10 },
                    { 5, "/images/products/dell.png", "Dell XPS 9640 U7-155H/32GB/1TB/W11P RTX 4060 8GB Laptop", 2699m, 30 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
