using Business.Contracts.Account;
using FluentValidation;

namespace Business.Validators.Account;

public class LoginUserValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(128);
    }
}