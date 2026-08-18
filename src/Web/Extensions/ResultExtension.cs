using Business.Common.Result;

namespace Web.Extensions;

public static class ResultExtension
{
    public static IResult ToMinimalApiResult<T>(this Result<T> result)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            return TypedResults.Ok(result.Value);
        }

        return ((Result)result).ToMinimalApiResult();
    }

    public static IResult ToMinimalApiResult(this BaseResult result)
    {
        var error = result.Error;
        if (error is null)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Json(
            data: new 
            {
                error = error.Message,
                errorCode = error.Code,
            },
            statusCode: (int)error.StatusCode
        );
    }
}