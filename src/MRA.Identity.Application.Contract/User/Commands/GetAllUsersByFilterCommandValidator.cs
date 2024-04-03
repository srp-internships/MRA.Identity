using FluentValidation;

namespace MRA.Identity.Application.Contract.User.Commands;

public class GetAllUsersByFilterCommandValidator : AbstractValidator<GetAllUsersByFilterCommand>
{
    public GetAllUsersByFilterCommandValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.ApplicationClientSecret).NotEmpty();
    }
}