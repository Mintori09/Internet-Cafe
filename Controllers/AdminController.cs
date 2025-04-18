using InternetCafeManagementSystem.Data;
using InternetCafeManagementSystem.Extensions;
using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace InternetCafeManagementSystem.Controllers;

[Authorize]
public class AdminController : BaseController
{
    private readonly ComputerDbContext _context;
    private readonly IUserService _userService;
    private readonly IComputerService _computerService;

    public AdminController(ComputerDbContext context, IUserService userService, IComputerService computerService)
    {
        _context = context;
        _userService = userService;
        _computerService = computerService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user == null || !user.IsAdmin)
        {
            return RedirectToAction("Index", "Home");
        }

        var computers = await _computerService.GetAllComputersAsync();
        var activeSessions = computers
            .SelectMany(c => c.UserSessions ?? new List<UserSession>())
            .Where(s => s.EndTime == null)
            .ToList();

        ViewBag.TotalRevenue = activeSessions.Sum(s => s.TotalCost);
        ViewBag.ActiveSessions = activeSessions.Count;
        ViewBag.TotalComputers = computers.Count;
        ViewBag.AvailableComputers = computers.Count(c => c.Status);

        return View(computers);
    }

    public async Task<IActionResult> Dashboard()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user == null || !user.IsAdmin)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);
        var lastWeek = today.AddDays(-7);
        var lastMonth = today.AddMonths(-1);

        ViewBag.TodayRevenue = await _computerService.GetTotalRevenueByDateRangeAsync(today, today.AddDays(1));
        ViewBag.YesterdayRevenue = await _computerService.GetTotalRevenueByDateRangeAsync(yesterday, today);
        ViewBag.LastWeekRevenue = await _computerService.GetTotalRevenueByDateRangeAsync(lastWeek, today);
        ViewBag.LastMonthRevenue = await _computerService.GetTotalRevenueByDateRangeAsync(lastMonth, today);

        ViewBag.TodaySessions = await _computerService.GetSessionsByDateRangeAsync(today, today.AddDays(1));
        ViewBag.HourlyStats = await _computerService.GetSessionCountByHourAsync(today);

        return View();
    }

    public async Task<IActionResult> RevenueByComputer()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user == null || !user.IsAdmin)
        {
            return RedirectToAction("Index", "Home");
        }

        var sessions = await _computerService.GetAllSessionsAsync();
        return View(sessions);
    }

    public async Task<IActionResult> RevenueByTime(DateTime? startDate, DateTime? endDate)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user == null || !user.IsAdmin)
        {
            return RedirectToAction("Index", "Home");
        }

        if (!startDate.HasValue)
        {
            startDate = DateTime.Today.AddDays(-7);
        }

        if (!endDate.HasValue)
        {
            endDate = DateTime.Today;
        }

        ViewBag.StartDate = startDate;
        ViewBag.EndDate = endDate;

        var sessions = await _computerService.GetSessionsByDateRangeAsync(startDate.Value, endDate.Value);
        return View(sessions);
    }

    public async Task<IActionResult> Statistics()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user == null || !user.IsAdmin)
        {
            return RedirectToAction("Index", "Home");
        }

        var computers = await _computerService.GetAllComputersAsync();
        var sessions = await _computerService.GetAllSessionsAsync();

        ViewBag.TotalRevenue = sessions.Sum(s => s.TotalCost);
        ViewBag.TotalSessions = sessions.Count;
        ViewBag.AverageSessionDuration = sessions.Average(s => s.EndTime.HasValue ? (s.EndTime.Value - s.StartTime).TotalHours : 0);
        ViewBag.MostUsedComputer = computers.OrderByDescending(c => c.UserSessions?.Count ?? 0).FirstOrDefault()?.Name;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetRevenueStatistics(DateTime startDate, DateTime endDate)
    {
        var sessions = await _userService.GetSessionsByDateRangeAsync(startDate, endDate);
        var revenueByComputer = await _userService.GetRevenueByComputerAsync(startDate, endDate);

        var computers = await _computerService.GetAllComputersAsync();
        var computerRevenues = computers.ToDictionary(
            c => c.Name,
            c => revenueByComputer.ContainsKey(c.Id) ? revenueByComputer[c.Id] : 0
        );

        ViewBag.Sessions = sessions;
        ViewBag.ComputerRevenues = computerRevenues;
        ViewBag.TotalRevenue = sessions.Sum(s => s.TotalCost);
        ViewBag.StartDate = startDate;
        ViewBag.EndDate = endDate;

        return View("Statistics");
    }
}