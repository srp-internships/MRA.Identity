using FluentValidation;

namespace MRA.Identity.Application.Contract.User.Commands.UsersByApplications;

public class GetUserByKeyCommandValidator : AbstractValidator<GetUserByKeyCommand>
{
    public GetUserByKeyCommandValidator()
    {
        RuleFor(x=> x.Key).NotEmpty();
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.ApplicationClientSecret).NotEmpty();
    }
}