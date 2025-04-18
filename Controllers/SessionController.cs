using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InternetCafeManagementSystem.Data;

namespace InternetCafeManagementSystem.Controllers;

[Authorize]
public class SessionController : BaseController
{
    private readonly IComputerService _computerService;
    private readonly IUserService _userService;
    private readonly ComputerDbContext _context;
    private readonly ILogger<SessionController> _logger;

    public SessionController(IComputerService computerService, IUserService userService, ComputerDbContext context, ILogger<SessionController> logger)
    {
        _computerService = computerService;
        _userService = userService;
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> StartSession(int computerId)
    {
        var user = await _userService.GetUserByIdAsync(GetCurrentUserId());
        if (user == null)
        {
            return NotFound();
        }

        // Kiểm tra xem người dùng đã có phiên sử dụng nào chưa
        var activeSession = await _computerService.GetActiveSessionAsync(user.Id, computerId);
        if (activeSession != null)
        {
            TempData["ErrorMessage"] = "Bạn đang có phiên sử dụng máy khác.";
            return RedirectToAction("Index", "Computer");
        }

        // Kiểm tra số dư tài khoản
        if (user.Balance <= 0)
        {
            TempData["ErrorMessage"] = "Số dư tài khoản không đủ. Vui lòng nạp thêm tiền.";
            return RedirectToAction("Profile", "Account");
        }

        var session = new UserSession
        {
            UserId = user.Id,
            ComputerId = computerId,
            StartTime = DateTime.Now,
            EndTime = null,
            TotalCost = 0
        };

        await _computerService.StartSessionAsync(session);
        TempData["SuccessMessage"] = "Bắt đầu sử dụng máy thành công.";
        return RedirectToAction("Index", "Computer");
    }

    [HttpPost]
    public async Task<IActionResult> EndSession(int computerId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var activeSession = await _context.UserSessions
            .Include(s => s.Computer)
            .FirstOrDefaultAsync(s => s.UserId == userId.Value
                && s.ComputerId == computerId
                && s.EndTime == null);

        if (activeSession == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy phiên sử dụng";
            return RedirectToAction("Index", "Computer");
        }

        try
        {
            await _computerService.EndSessionAsync(activeSession);
            TempData["SuccessMessage"] = "Đã kết thúc phiên sử dụng";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending session for user {UserId} on computer {ComputerId}", userId, computerId);
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi kết thúc phiên sử dụng";
        }

        return RedirectToAction("Index", "Computer");
    }

    public async Task<IActionResult> MySessions()
    {
        var user = await _userService.GetUserByIdAsync(GetCurrentUserId());
        if (user == null)
        {
            return NotFound();
        }

        var sessions = await _computerService.GetUserSessionsAsync(user.Id);
        return View(sessions);
    }
}