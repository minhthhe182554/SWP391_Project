using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SWP391_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityColumnToUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Domains",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Domains",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Domains",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Companies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                defaultValue: "default_yvl9oh",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Companies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldDefaultValue: "default_yvl9oh");

            migrationBuilder.InsertData(
                table: "Domains",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "IT Phần mềm" },
                    { 2, "Marketing" },
                    { 3, "Sales" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "City", "Ward" },
                values: new object[,]
                {
                    { 1, "Hà Nội", "Cầu Giấy" },
                    { 2, "Hồ Chí Minh", "Quận 1" },
                    { 3, "Đà Nẵng", "Hải Châu" }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Java" },
                    { 2, "C#" },
                    { 3, ".NET Core" },
                    { 4, "ReactJS" },
                    { 5, "SQL Server" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "Email", "Password", "Role" },
                values: new object[,]
                {
                    { 1, true, "admin@ezjob.com", "123456", 2 },
                    { 2, true, "recruiter@fpt.com", "123456", 1 },
                    { 3, true, "ungvien@gmail.com", "123456", 0 }
                });

            migrationBuilder.InsertData(
                table: "Candidates",
                columns: new[] { "Id", "FullName", "ImageUrl", "Jobless", "PhoneNumber", "RemainingReport", "UserId" },
                values: new object[] { 1, "Nguyễn Văn A", null, true, "0912345678", 2, 3 });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Address", "Description", "ImageUrl", "LocationId", "Name", "PhoneNumber", "UserId", "Website" },
                values: new object[] { 1, "Số 17 Duy Tân", "Công ty công nghệ hàng đầu Việt Nam", null, 1, "FPT Software", "0988888888", 2, "https://fpt-software.com" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "Address", "CompanyId", "Description", "EndDate", "HigherSalaryRange", "IsDelete", "LocationId", "LowerSalaryRange", "StartDate", "Title", "Type", "VacancyCount", "YearsOfExperience" },
                values: new object[] { 1, null, 1, "Tham gia phát triển dự án Banking...", new DateTime(2026, 1, 7, 23, 44, 21, 958, DateTimeKind.Local).AddTicks(8510), 30000000m, false, 1, 15000000m, new DateTime(2025, 12, 8, 23, 44, 21, 958, DateTimeKind.Local).AddTicks(8460), "Tuyển dụng Senior .NET Developer", 0, 5, 2 });
        }
    }
}
