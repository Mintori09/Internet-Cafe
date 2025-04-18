using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;

namespace InternetCafeManagementSystem.Migrations;

public class Computer
{
    public partial class InitialCreate : DbLoggerCategory.Migrations
    {
        protected void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Computers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "Boolean", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Computers", x => x.Id); // ✅ Đặt tên cho Primary Key
                });
        }
    }
    protected void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Computers");
    }
}