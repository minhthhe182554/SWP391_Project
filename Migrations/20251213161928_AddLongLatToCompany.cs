using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWP391_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddLongLatToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Companies",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Companies",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Companies");
        }
    }
}
