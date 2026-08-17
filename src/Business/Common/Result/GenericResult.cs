namespace Business.Common.Result;

public class Result<T> : BaseResult
{
    private readonly T? _value;
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Can not access Value of a failed result.");

    public Result(T? value, Error? error = null, Dictionary<string, string>? details = null) : base(error, details)
    {
        _value = value;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(default, error);
}