using System.Security.Claims;
using Data.Enums;
using Data.Interfaces.Repositories;
using Microsoft.AspNetCore.Authentication;

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
                    await context.SignOutAsync();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        errorCode = "USER_BLOCKED",
                        error = "User is blocked"
                    });

                    return; 
                }
                else if (userStatus is null)
                {
                    await context.SignOutAsync();
                }
            }
        }

        await _next(context);
    }
}
