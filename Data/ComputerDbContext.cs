using InternetCafeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InternetCafeManagementSystem.Data;

public class ComputerDbContext : DbContext
{
    public ComputerDbContext(DbContextOptions<ComputerDbContext> options) : base(options)
    {
        
    }
    public DbSet<Computer> Computers { get; set; }
    
}
