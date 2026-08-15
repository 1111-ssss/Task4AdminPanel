using Data.Enums;
using Business.Contracts.Account;
using Business.Interfaces.Account;
using Microsoft.AspNetCore.Mvc;
using Ardalis.Result.AspNetCore;

namespace Web.Endpoints;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/account");

        group.MapPost("/register", Register);
        
        return group;
    }

    private static async Task<IResult> Register(
        [FromServices] IAccountRegisterService accountRegisterService,
        RegisterUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await accountRegisterService.Register(request);

        return result.ToMinimalApiResult();
    }
}