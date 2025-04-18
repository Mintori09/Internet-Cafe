using InternetCafeManagementSystem.Data;
using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace InternetCafeManagementSystem.Controllers;

[AllowAnonymous]
public class AccountController : BaseController
{
    private readonly IUserService _userService;
    private readonly ILogger<AccountController> _logger;
    private readonly ComputerDbContext _context;

    public AccountController(IUserService userService, ILogger<AccountController> logger, ComputerDbContext context)
    {
        _userService = userService;
        _logger = logger;
        _context = context;
    }

    public IActionResult Login()
    {
        // If user is already logged in, redirect to home
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        try
        {
            var user = await _userService.LoginAsync(username, password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetInt32("IsAdmin", user.IsAdmin ? 1 : 0);

                _logger.LogInformation("User {Username} logged in successfully", username);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", username);
            ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng nhập");
            return View();
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.RegisterAsync(user);
            if (result)
            {
                return RedirectToAction("Login");
            }
            ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
        }
        return View(user);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddBalance(decimal amount)
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();

        var success = await _userService.UpdateBalanceAsync(userId, amount);
        if (!success) return BadRequest("Không thể cập nhật số dư");

        return RedirectToAction("Profile");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        // Lấy lịch sử giao dịch
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(10)
            .ToListAsync();

        // Lấy thông tin phiên sử dụng hiện tại
        var currentSession = await _context.UserSessions
            .Include(s => s.Computer)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.EndTime == null);

        ViewBag.Transactions = transactions;
        ViewBag.CurrentSession = currentSession;

        return View(user);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> Deposit(int amount)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("Login");
        }

        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        if (amount <= 0)
        {
            TempData["ErrorMessage"] = "Số tiền nạp phải lớn hơn 0";
            return RedirectToAction("Profile");
        }

        user.Balance += amount;
        await _userService.UpdateUserAsync(user);

        // Lưu lịch sử giao dịch
        var transaction = new Transaction
        {
            UserId = user.Id,
            Amount = amount,
            Type = TransactionType.Deposit,
            Description = "Nạp tiền vào tài khoản",
            CreatedAt = DateTime.Now
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Đã nạp {amount:N0} VNĐ vào tài khoản thành công";
        return RedirectToAction("Profile");
    }
}