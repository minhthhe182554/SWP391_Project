using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWP391_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Jobs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "EndDate", "StartDate" },
                values: new object[] { null, new DateTime(2026, 1, 7, 23, 44, 21, 958, DateTimeKind.Local).AddTicks(8510), new DateTime(2025, 12, 8, 23, 44, 21, 958, DateTimeKind.Local).AddTicks(8460) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Jobs");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 1, 7, 23, 39, 28, 772, DateTimeKind.Local).AddTicks(30), new DateTime(2025, 12, 8, 23, 39, 28, 771, DateTimeKind.Local).AddTicks(9990) });
        }
    }
}
