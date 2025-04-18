using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetCafeManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ComputerId1",
                table: "UserSessions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_ComputerId1",
                table: "UserSessions",
                column: "ComputerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Computers_Code",
                table: "Computers",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessions_Computers_ComputerId1",
                table: "UserSessions",
                column: "ComputerId1",
                principalTable: "Computers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSessions_Computers_ComputerId1",
                table: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_UserSessions_ComputerId1",
                table: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Computers_Code",
                table: "Computers");

            migrationBuilder.DropColumn(
                name: "ComputerId1",
                table: "UserSessions");
        }
    }
}
