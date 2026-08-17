using System.Security.Claims;
using Data.Common.Enums;
using Data.Interfaces.Repositories;

namespace Web.Middleware;

public class UserStatusCheckMiddleware
{
    private readonly RequestDelegate _next;

    public UserStatusCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userIdClaim))
            {
                var userStatus = await userRepository.GetUserStatusByEmail(userIdClaim);

                if (userStatus == UserStatus.Blocked)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new { 
                        code = "USER_BLOCKED", 
                        message = "User is blocked"
                    });
                    return;
                }
            }
        }

        await _next(context);
    }
}
