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

                    context.Response.Redirect("/login?blocked=1");
                    return; 
                }
            }
        }

        await _next(context);
    }
}
