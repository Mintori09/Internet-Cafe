using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetCafeManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryComputerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMaintenanceDate",
                table: "Computers");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Computers");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Computers");

            migrationBuilder.DropColumn(
                name: "Specifications",
                table: "Computers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastMaintenanceDate",
                table: "Computers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Computers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Computers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Specifications",
                table: "Computers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
