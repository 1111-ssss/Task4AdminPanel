using Ardalis.Result.AspNetCore;
using Business.Contracts.Admin;
using Business.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Web.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/admin");

        group.MapGet("/users", ListUsers)
            .RequireAuthorization();

        group.MapDelete("/users/{email}", DeleteUser)
            .RequireAuthorization();

        group.MapPost("/users/{email}/block", BlockUser)
            .RequireAuthorization();

        group.MapPost("/users/{email}/unblock", UnblockUser)
            .RequireAuthorization();

        return group;
    }

    private static async Task<IResult> ListUsers(
        [FromServices] IListUsersService listUsersService,
        [AsParameters] ListUsersRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await listUsersService.ListUsers(request);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> DeleteUser(
        [FromServices] IDeleteUserService deleteUserService,
        [FromRoute] string email,
        CancellationToken cancellationToken
    )
    {
        var result = await deleteUserService.DeleteUser(new UserRequest(email));

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> BlockUser(
        [FromServices] IBlockUserService blockUserService,
        [FromRoute] string email,
        CancellationToken cancellationToken
    )
    {
        var result = await blockUserService.BlockUser(new UserRequest(email));

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> UnblockUser(
        [FromServices] IBlockUserService unblockUserService,
        [FromRoute] string email,
        CancellationToken cancellationToken
    )
    {
        var result = await unblockUserService.UnblockUser(new UserRequest(email));

        return result.ToMinimalApiResult();
    }
}