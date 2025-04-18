using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InternetCafeManagementSystem.Data;
using System.Security.Claims;

namespace InternetCafeManagementSystem.Authorization;

public static class AuthorizationPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string UserOnly = "UserOnly";
    public const string SessionOwner = "SessionOwner";
    public const string ManageComputers = "ManageComputers";
    public const string ManageSessions = "ManageSessions";

    public static void ConfigurePolicies(AuthorizationOptions options)
    {
        // Admin policy - chỉ admin mới có thể quản lý
        options.AddPolicy(AdminOnly, policy =>
            policy.RequireRole("Admin"));

        // User policy - chỉ user mới có thể sử dụng máy
        options.AddPolicy(UserOnly, policy =>
            policy.RequireRole("User"));

        // Session owner policy - chỉ chủ phiên mới có thể kết thúc phiên
        options.AddPolicy(SessionOwner, policy =>
            policy.RequireAssertion(context =>
            {
                var sessionId = context.Resource as int?;
                if (!sessionId.HasValue)
                    return false;

                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return false;

                var dbContext = context.Resource as ComputerDbContext;
                if (dbContext == null)
                    return false;

                var session = dbContext.UserSessions
                    .FirstOrDefault(s => s.Id == sessionId.Value && s.UserId.ToString() == userId);

                return session != null;
            }));

        // Manage computers policy
        options.AddPolicy(ManageComputers, policy =>
            policy.RequireRole("Admin"));

        // Manage sessions policy
        options.AddPolicy(ManageSessions, policy =>
            policy.RequireRole("Admin"));
    }
}