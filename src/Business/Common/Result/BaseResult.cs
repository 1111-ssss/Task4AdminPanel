namespace Business.Common.Result;

public abstract class BaseResult
{
    public bool IsSuccess { get => Error is null; }
    public Error? Error { get; }
    public Dictionary<string, string>? Details { get; }

    protected BaseResult(Error? error = null, Dictionary<string, string>? details = null)
    {
        Error = error;
        Details = details;
    }
}