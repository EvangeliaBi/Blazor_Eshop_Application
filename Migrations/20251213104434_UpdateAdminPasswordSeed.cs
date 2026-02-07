using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminPasswordSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminPasswordHash",
                value: "AQAAAAIAAYagAAAAEKw4201BnMtft9Iz+/LKObjQQtYQfKzEAmGsCa/OTkcQsZVTFv3XR4hACZ2Rtei9Og==");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminPasswordHash",
                value: "AQAAAAIAAYagAAAAEHyQVL2GA9mtR7BideC77KIazRdMhPpNYdYREjHm8sPn1ua0oZb7WcZp9RF4JuYk9w==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminPasswordHash",
                value: "Tech1");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminPasswordHash",
                value: "Tech2");
        }
    }
}
