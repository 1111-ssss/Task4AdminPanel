using Business.Common.Result;
using FluentValidation;

namespace Business.Services;

public abstract class ServiceValidation
{
    protected async Task<Result> Validate<T>(IValidator<T> validator, T request, CancellationToken cancellationToken)
    {
        if (validator == null) {
            return Result.Success();
        }

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.AsError());
        }

        return Result.Success();
    }
}