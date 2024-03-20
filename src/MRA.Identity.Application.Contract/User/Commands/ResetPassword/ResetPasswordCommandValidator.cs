using FluentValidation;
using MRA.Identity.Application.Contract.ContentService;

namespace MRA.Identity.Application.Contract.User.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator(IContentService contentService)
    {
        RuleFor(s => s.Password).NotEmpty().MinimumLength(5);
        RuleFor(sm => sm.PhoneNumber).NotEmpty()
            .Matches(@"^(?:\d{9}|\+992\d{9}|992\d{9})$")
            .WithMessage(contentService["PhoneNumberMessage"]);
        RuleFor(s => s.ConfirmPassword).NotEmpty()
            .Equal(s => s.Password)
            .WithMessage(contentService["ConfirmPasswordMessage"]);
    }
}