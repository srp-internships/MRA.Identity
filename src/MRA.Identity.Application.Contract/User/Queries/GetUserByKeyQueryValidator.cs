using FluentValidation;

namespace MRA.Identity.Application.Contract.User.Queries;

public class GetUserByKeyQueryValidator : AbstractValidator<GetUserByKeyQuery>
{
    public GetUserByKeyQueryValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.ApplicationClientSecret).NotEmpty();
    }
}