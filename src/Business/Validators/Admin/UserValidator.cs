using Business.Contracts.Admin;
using FluentValidation;

namespace Business.Validators.Admin;

public class UserValidator : AbstractValidator<UserRequest>
{
    public UserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}