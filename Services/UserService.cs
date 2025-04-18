using InternetCafeManagementSystem.Data;
using InternetCafeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace InternetCafeManagementSystem.Services;

public class UserService : IUserService
{
    private readonly ComputerDbContext _context;

    public UserService(ComputerDbContext context)
    {
        _context = context;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
            return null;

        // Try both hashed and plain text password
        if (user.Password == password || VerifyPassword(password, user.Password))
            return user;

        return null;
    }

    public async Task<bool> RegisterAsync(User user)
    {
        try
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return false;

            user.Password = HashPassword(user.Password);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateBalanceAsync(int userId, decimal amount)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.Balance += amount;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetUserBalanceAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user?.Balance ?? 0;
    }

    public async Task<List<UserSession>> GetUserSessionsAsync(int userId)
    {
        return await _context.UserSessions
            .Include(us => us.Computer)
            .Where(us => us.UserId == userId)
            .OrderByDescending(us => us.StartTime)
            .ToListAsync();
    }

    public async Task<List<UserSession>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.UserSessions
            .Include(us => us.User)
            .Include(us => us.Computer)
            .Where(us => us.StartTime >= startDate && us.StartTime <= endDate)
            .OrderByDescending(us => us.StartTime)
            .ToListAsync();
    }

    public async Task<Dictionary<int, decimal>> GetRevenueByComputerAsync(DateTime startDate, DateTime endDate)
    {
        var sessions = await _context.UserSessions
            .Where(us => us.StartTime >= startDate && us.StartTime <= endDate)
            .GroupBy(us => us.ComputerId)
            .Select(g => new
            {
                ComputerId = g.Key,
                TotalRevenue = g.Sum(us => us.TotalCost)
            })
            .ToListAsync();

        return sessions.ToDictionary(s => s.ComputerId, s => s.TotalRevenue);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPassword(string inputPassword, string hashedPassword)
    {
        return HashPassword(inputPassword) == hashedPassword;
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}