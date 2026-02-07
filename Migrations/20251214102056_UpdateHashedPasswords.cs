using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHashedPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENrC26B5B3NUJz1gHkP/L71m+TmRydBEBYpOCST+NypUHYejCsZc/f2nTB+lnGRCKg==");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFEXGCpv9CkS5/MUG3ltvJ6rss2+59AhrPgo+ekuUbJiASWTpOdVEal8HdX6lh5kKg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEKA4StxvHKZ/9HZVU7U8nYaohjj8gHWpsqeH2pGwmDFP9LnM/+yHwVEgvqj/fq5YJw==");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEHpA5i4C3mUbvDSppzhDVYPspdZT6v/Aocilx89V2YIcZH1eArCNxxCU/y7fbjBugg==");
        }
    }
}
