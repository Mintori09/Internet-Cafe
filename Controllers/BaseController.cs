using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InternetCafeManagementSystem.Controllers;

[Authorize]
public class BaseController : Controller
{
    protected int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }

    protected string GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }

    protected bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    protected IActionResult RedirectToLogin()
    {
        return RedirectToAction("Login", "Account");
    }

    protected IActionResult RedirectToAccessDenied()
    {
        return RedirectToAction("AccessDenied", "Account");
    }
}