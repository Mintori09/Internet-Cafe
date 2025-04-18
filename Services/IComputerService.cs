using InternetCafeManagementSystem.Models;

namespace InternetCafeManagementSystem.Services;

public interface IComputerService
{
    Task<List<Computer>> GetAllComputersAsync();
    Task<Computer?> GetComputerByIdAsync(int id);
    Task AddComputerAsync(Computer computer);
    Task UpdateComputerAsync(Computer computer);
    Task DeleteComputerAsync(int id);
    Task StartSessionAsync(UserSession session);
    Task EndSessionAsync(UserSession session);
    Task<UserSession?> GetActiveSessionAsync(int userId, int computerId);
    Task<List<UserSession>> GetAllSessionsAsync();
    Task<List<UserSession>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Dictionary<int, decimal>> GetRevenueByComputerAsync(DateTime startDate, DateTime endDate);
    Task<Dictionary<string, int>> GetSessionCountByHourAsync(DateTime date);
    Task<List<UserSession>> GetUserSessionsAsync(int userId);
}