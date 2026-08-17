namespace Data.Common.Result;

public class Result : BaseResult
{
    protected Result(Error? error = null) : base(error) { }

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);
}