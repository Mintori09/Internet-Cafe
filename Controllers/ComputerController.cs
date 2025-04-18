using InternetCafeManagementSystem.Data;
using InternetCafeManagementSystem.Models;
using InternetCafeManagementSystem.Services;
using InternetCafeManagementSystem.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternetCafeManagementSystem.Controllers;

[Authorize]
public class ComputerController : BaseController
{
    private readonly IComputerService _computerService;
    private readonly IUserService _userService;
    private readonly ComputerDbContext _context;

    public ComputerController(IComputerService computerService, IUserService userService, ComputerDbContext context)
    {
        _computerService = computerService;
        _userService = userService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var computers = await _computerService.GetAllComputersAsync();
        var userId = GetCurrentUserId();

        if (userId != null)
        {
            var activeSessions = await _computerService.GetUserSessionsAsync(userId);
            ViewBag.ActiveSessions = activeSessions.Where(s => s.EndTime == null).ToList();
        }

        return View(computers);
    }

    // GET: Computer/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var computer = await _computerService.GetComputerByIdAsync(id);
        if (computer == null)
        {
            return NotFound();
        }
        return View(computer);
    }

    // GET: Computer/Create
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Computer/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(Computer computer)
    {
        if (ModelState.IsValid)
        {
            await _computerService.AddComputerAsync(computer);
            TempData["SuccessMessage"] = "Thêm máy tính thành công.";
            return RedirectToAction(nameof(Index));
        }
        return View(computer);
    }

    // GET: Computer/Edit/5
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Edit(int id)
    {
        var computer = await _computerService.GetComputerByIdAsync(id);
        if (computer == null)
        {
            return NotFound();
        }
        return View(computer);
    }

    // POST: Computer/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Edit(int id, Computer computer)
    {
        if (id != computer.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _computerService.UpdateComputerAsync(computer);
            TempData["SuccessMessage"] = "Cập nhật máy tính thành công.";
            return RedirectToAction(nameof(Index));
        }
        return View(computer);
    }

    // GET: Computer/Delete/5
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var computer = await _computerService.GetComputerByIdAsync(id);
        if (computer == null)
        {
            return NotFound();
        }
        return View(computer);
    }

    // POST: Computer/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _computerService.DeleteComputerAsync(id);
        TempData["SuccessMessage"] = "Xóa máy tính thành công.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Computer/Use/5
    public async Task<IActionResult> Use(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var computer = await _context.Computers.FindAsync(id);
        if (computer == null)
        {
            return NotFound();
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Check if computer is available
        if (!computer.Status)
        {
            TempData["Message"] = "This computer is currently in use.";
            return RedirectToAction(nameof(Index));
        }

        // Check if user has enough balance
        if (user.Balance <= 0)
        {
            TempData["Message"] = "You need to add money to your account first.";
            return RedirectToAction(nameof(Index));
        }

        // Create new session
        var session = new UserSession
        {
            UserId = user.Id,
            ComputerId = computer.Id,
            StartTime = DateTime.Now,
            TotalCost = 0
        };

        // Update computer status
        computer.Status = false;

        _context.UserSessions.Add(session);
        _context.Computers.Update(computer);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> StartSession(int computerId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user == null || !user.IsActive)
        {
            return RedirectToAction("Login", "Account");
        }

        var computer = await _computerService.GetComputerByIdAsync(computerId);
        if (computer == null || !computer.Status)
        {
            TempData["ErrorMessage"] = "Máy tính không khả dụng.";
            return RedirectToAction("Index");
        }

        var activeSession = await _computerService.GetActiveSessionAsync(userId.Value, computerId);
        if (activeSession != null)
        {
            TempData["ErrorMessage"] = "Bạn đang có phiên sử dụng máy này.";
            return RedirectToAction("Index");
        }

        var session = new UserSession
        {
            UserId = userId.Value,
            ComputerId = computerId,
            StartTime = DateTime.Now
        };

        await _computerService.StartSessionAsync(session);
        TempData["SuccessMessage"] = "Đã bắt đầu phiên sử dụng.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> EndSession(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user == null || !user.IsActive)
        {
            return RedirectToAction("Login", "Account");
        }

        var computer = await _computerService.GetComputerByIdAsync(id);
        if (computer == null)
        {
            return NotFound();
        }

        var session = await _computerService.GetActiveSessionAsync(userId.Value, id);
        if (session == null)
        {
            TempData["ErrorMessage"] = "No active session found for this computer.";
            return RedirectToAction(nameof(Index));
        }

        var endTime = DateTime.Now;
        session.EndTime = endTime;
        var duration = (endTime - session.StartTime).TotalHours;
        session.TotalCost = (decimal)(duration * (double)computer.PricePerHour);

        if (user.Balance < session.TotalCost)
        {
            TempData["ErrorMessage"] = "Insufficient balance to end session.";
            return RedirectToAction(nameof(Index));
        }

        user.Balance -= session.TotalCost;
        await _userService.UpdateUserAsync(user);
        await _computerService.EndSessionAsync(session);

        TempData["SuccessMessage"] = $"Session ended. Total cost: {session.TotalCost:C0}";
        return RedirectToAction(nameof(Index));
    }

    [AcceptVerbs("Get", "Post")]
    public async Task<IActionResult> VerifyComputerName(string name, int id = 0)
    {
        var computer = await _context.Computers.FirstOrDefaultAsync(c => c.Name == name && c.Id != id);
        if (computer != null)
        {
            return Json($"Tên máy {name} đã tồn tại");
        }
        return Json(true);
    }

    private bool ComputerExists(int id)
    {
        return _context.Computers.Any(e => e.Id == id);
    }
}