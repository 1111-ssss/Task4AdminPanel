namespace Business.Common.Result;

public class Result : BaseResult
{
    public Result(Error? error = null, Dictionary<string, string>? details = null) : base(error, details) { }

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);
}