using System.Net;
using FluentValidation.Results;

namespace Business.Common.Result;

public static class ValidationResultExtensions
{
    public static Error AsError(this ValidationResult validationResult)
    {
        var firstFailure = validationResult.Errors.First();

        return new Error(
            StatusCode: HttpStatusCode.BadRequest,
            Code: firstFailure.ErrorCode ?? "ValidationError",
            Message: firstFailure.ErrorMessage
        );
    }
}