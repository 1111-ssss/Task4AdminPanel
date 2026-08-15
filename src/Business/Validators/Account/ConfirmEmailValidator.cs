using Business.Contracts.Account;
using FluentValidation;

namespace Business.Validators.Account;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .MaximumLength(256);
    }
}