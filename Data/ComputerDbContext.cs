using InternetCafeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InternetCafeManagementSystem.Data;

public class ComputerDbContext : DbContext
{
    public ComputerDbContext(DbContextOptions<ComputerDbContext> options) : base(options)
    {
    }

    public DbSet<Computer> Computers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure UserSession relationships
        modelBuilder.Entity<UserSession>()
            .HasOne(us => us.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserSession>()
            .HasOne(us => us.Computer)
            .WithMany(c => c.Sessions)
            .HasForeignKey(us => us.ComputerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Computer
        modelBuilder.Entity<Computer>()
            .Property(c => c.PricePerHour)
            .HasPrecision(18, 2);

        // Configure UserSession
        modelBuilder.Entity<UserSession>()
            .Property(us => us.TotalCost)
            .HasPrecision(18, 2);

        // Configure User
        modelBuilder.Entity<User>()
            .Property(u => u.Balance)
            .HasPrecision(18, 2);

        // Ensure unique indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId);
    }
}
