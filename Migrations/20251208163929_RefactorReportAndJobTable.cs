using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWP391_Project.Migrations
{
    /// <inheritdoc />
    public partial class RefactorReportAndJobTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkTime",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Reports",
                newName: "Reason");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2026, 1, 7, 23, 39, 28, 772, DateTimeKind.Local).AddTicks(30), new DateTime(2025, 12, 8, 23, 39, 28, 771, DateTimeKind.Local).AddTicks(9990) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Reports",
                newName: "Content");

            migrationBuilder.AddColumn<string>(
                name: "WorkTime",
                table: "Jobs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "StartDate", "WorkTime" },
                values: new object[] { new DateTime(2026, 1, 3, 19, 18, 27, 683, DateTimeKind.Local).AddTicks(5062), new DateTime(2025, 12, 4, 19, 18, 27, 683, DateTimeKind.Local).AddTicks(5051), "Full-time (T2-T6)" });
        }
    }
}
