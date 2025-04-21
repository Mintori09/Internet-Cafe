using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetCafeManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Computers_Code",
                table: "Computers");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Computers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Computers",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Computers_Code",
                table: "Computers",
                column: "Code",
                unique: true);
        }
    }
}
