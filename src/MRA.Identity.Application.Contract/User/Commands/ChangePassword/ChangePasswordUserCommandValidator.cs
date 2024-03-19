using FluentValidation;
using MRA.Identity.Application.Contract.ContentService;

namespace MRA.Identity.Application.Contract.User.Commands.ChangePassword;

public class ChangePasswordUserCommandValidator : AbstractValidator<ChangePasswordUserCommand>
{
    public ChangePasswordUserCommandValidator(IContentService contentService)
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(5);
        RuleFor(x => x.ConfirmPassword).NotEmpty()
            .Equal(x => x.NewPassword)
            .WithMessage(contentService["ConfirmPasswordMessage"]);
    }
}