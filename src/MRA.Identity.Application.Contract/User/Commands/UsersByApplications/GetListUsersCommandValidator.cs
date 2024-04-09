using FluentValidation;

namespace MRA.Identity.Application.Contract.User.Commands.UsersByApplications;

public class GetListUsersCommandValidator : AbstractValidator<GetListUsersCommand>
{
    public GetListUsersCommandValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.ApplicationClientSecret).NotEmpty();
    }
}