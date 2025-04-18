using InternetCafeManagementSystem.Data;
using InternetCafeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InternetCafeManagementSystem.Services;

public class ComputerService : IComputerService
{
    private readonly ComputerDbContext _context;

    public ComputerService(ComputerDbContext context)
    {
        _context = context;
    }

    public async Task<List<Computer>> GetAllComputersAsync()
    {
        return await _context.Computers
            .Include(c => c.UserSessions)
            .ThenInclude(us => us.User)
            .ToListAsync();
    }

    public async Task<Computer?> GetComputerByIdAsync(int id)
    {
        return await _context.Computers
            .Include(c => c.UserSessions)
            .ThenInclude(us => us.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddComputerAsync(Computer computer)
    {
        _context.Computers.Add(computer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateComputerAsync(Computer computer)
    {
        _context.Computers.Update(computer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteComputerAsync(int id)
    {
        var computer = await GetComputerByIdAsync(id);
        if (computer != null)
        {
            _context.Computers.Remove(computer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<UserSession>> GetUserSessionsAsync(int userId)
    {
        return await _context.UserSessions
            .Include(us => us.Computer)
            .Where(us => us.UserId == userId)
            .OrderByDescending(us => us.StartTime)
            .ToListAsync();
    }

    public async Task<UserSession?> GetActiveSessionAsync(int userId, int computerId)
    {
        return await _context.UserSessions
            .FirstOrDefaultAsync(us => us.UserId == userId && us.ComputerId == computerId && us.EndTime == null);
    }

    public async Task StartSessionAsync(UserSession session)
    {
        var computer = await GetComputerByIdAsync(session.ComputerId);
        if (computer != null)
        {
            computer.Status = false;
            _context.Computers.Update(computer);
            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();
        }
    }

    public async Task EndSessionAsync(UserSession session)
    {
        session.EndTime = DateTime.Now;
        var duration = session.EndTime.Value - session.StartTime;
        session.TotalCost = (decimal)duration.TotalHours * session.Computer.PricePerHour;

        if (session.Computer != null)
        {
            session.Computer.Status = true;
            _context.Computers.Update(session.Computer);
        }

        _context.UserSessions.Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserSession>> GetAllSessionsAsync()
    {
        return await _context.UserSessions
            .Include(s => s.Computer)
            .Include(s => s.User)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<List<UserSession>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.UserSessions
            .Include(s => s.Computer)
            .Include(s => s.User)
            .Where(s => s.StartTime.Date >= startDate.Date &&
                       s.StartTime.Date <= endDate.Date)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.UserSessions
            .Where(s => s.StartTime >= startDate && s.StartTime <= endDate)
            .SumAsync(s => s.TotalCost);
    }

    public async Task<Dictionary<int, decimal>> GetRevenueByComputerAsync(DateTime startDate, DateTime endDate)
    {
        var revenues = await _context.UserSessions
            .Where(s => s.StartTime >= startDate && s.StartTime <= endDate)
            .GroupBy(s => s.ComputerId)
            .Select(g => new { ComputerId = g.Key, Revenue = g.Sum(s => s.TotalCost) })
            .ToListAsync();

        return revenues.ToDictionary(r => r.ComputerId, r => r.Revenue);
    }

    public async Task<Dictionary<string, int>> GetSessionCountByHourAsync(DateTime date)
    {
        var sessions = await _context.UserSessions
            .Where(s => s.StartTime.Date == date.Date)
            .ToListAsync();

        var hourlyCounts = new Dictionary<string, int>();
        for (int hour = 0; hour < 24; hour++)
        {
            var count = sessions.Count(s => s.StartTime.Hour == hour);
            hourlyCounts.Add($"{hour:00}:00", count);
        }

        return hourlyCounts;
    }
}
