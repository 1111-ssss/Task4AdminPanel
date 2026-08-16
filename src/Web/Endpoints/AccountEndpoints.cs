using Business.Contracts.Account;
using Business.Interfaces.Account;
using Microsoft.AspNetCore.Mvc;
using Ardalis.Result.AspNetCore;
using Web.Interfaces;

namespace Web.Endpoints;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/account");

        group.MapPost("/register", Register);
        group.MapPost("/login", Login);
        group.MapPost("/logout", Logout);
        group.MapPost("/confirm-email", ConfirmEmail);
        
        return group;
    }

    private static async Task<IResult> Register(
        [FromServices] IAccountRegisterService accountRegisterService,
        [FromServices] IEmailSenderService emailSenderService,
        RegisterUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await accountRegisterService.Register(request);

        if (result.IsSuccess)
        {
            await emailSenderService.SendConfirmationEmail(result.Value.Email);
        }

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> Login(
        [FromServices] IAccountLoginService accountLoginService,
        [FromServices] IAuthCookieService authCookieService,
        LoginUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await accountLoginService.Login(request);

        if (result.IsSuccess)
        {
            var user = result.Value;
            await authCookieService.SignIn(
                email: user.Email,
                name: user.Name,
                surname: user.Surname,
                status: user.Status,
                rememberMe: request.RememberMe
            );
        }

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> Logout(
        [FromServices] IAuthCookieService authCookieService,
        CancellationToken cancellationToken
    )
    {
        await authCookieService.SignOut();

        return TypedResults.NoContent();
    }

    private static async Task<IResult> ConfirmEmail(
        [FromServices] IConfirmEmailService accountConfirmEmailService,
        [FromServices] IAuthCookieService authCookieService,
        ConfirmEmailRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await accountConfirmEmailService.ConfirmEmail(request);

        if (result.IsSuccess)
        {
            await authCookieService.SignIn(
                email: result.Value.Email,
                name: result.Value.Name,
                surname: result.Value.Surname,
                status: result.Value.Status,
                rememberMe: false
            );
        }

        return result.ToMinimalApiResult();
    }
}