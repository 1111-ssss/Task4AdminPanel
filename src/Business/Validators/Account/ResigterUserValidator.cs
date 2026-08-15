using Business.Contracts.Account;
using FluentValidation;

namespace Business.Validators.Account;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(256);
        RuleFor(x => x.Surname)
            .NotEmpty()
            .MaximumLength(256);
    }
}