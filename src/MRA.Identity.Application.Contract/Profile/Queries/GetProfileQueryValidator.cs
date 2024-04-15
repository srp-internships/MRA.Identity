using FluentValidation;

namespace MRA.Identity.Application.Contract.Profile.Queries;

public class GetProfileQueryValidator : AbstractValidator<GetProfileQuery>
{
    public GetProfileQueryValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.ApplicationClientSecret).NotEmpty();
    }
}