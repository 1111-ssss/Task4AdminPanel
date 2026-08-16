using Business.Contracts.Admin;
using FluentValidation;

namespace Business.Validators.Admin;

public class ListUsersValidator : AbstractValidator<ListUsersRequest>
{
    public ListUsersValidator()
    {
        RuleFor(x => x.Page)
            .NotNull()
            .GreaterThan(0);
        RuleFor(x => x.PageSize)
            .NotNull()
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
        RuleFor(x => x.Search)
            .MaximumLength(256);
        RuleFor(x => x.OrderBy)
            .MaximumLength(256);
        RuleFor(x => x.IsAsc)
            .NotNull();
    }
}