using FluentValidation;

namespace MRA.Identity.Application.Contract.User.Commands.UsersByApplications;

public class GetPagedListUsersCommandValidator : AbstractValidator<GetPagedListUsersCommand>
{
    public GetPagedListUsersCommandValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.ApplicationClientSecret).NotEmpty();
    }
}