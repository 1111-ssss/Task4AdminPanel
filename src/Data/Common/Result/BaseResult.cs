namespace Data.Common.Result;

public abstract class BaseResult
{
    public bool IsSuccess { get => Error is null; }
    public Error? Error { get; }

    protected BaseResult(Error? error = null)
    {
        Error = error;
    }
}