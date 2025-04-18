using InternetCafeManagementSystem.Models;

namespace InternetCafeManagementSystem.Services;

public interface IUserService
{
    Task<User?> LoginAsync(string username, string password);
    Task<bool> RegisterAsync(User user);
    Task<bool> UpdateBalanceAsync(int userId, decimal amount);
    Task<decimal> GetUserBalanceAsync(int userId);
    Task<List<UserSession>> GetUserSessionsAsync(int userId);
    Task<List<UserSession>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Dictionary<int, decimal>> GetRevenueByComputerAsync(DateTime startDate, DateTime endDate);
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
}