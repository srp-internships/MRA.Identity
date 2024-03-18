﻿using FluentValidation;

namespace MRA.Identity.Application.Contract.User.Commands.ChangePassword;

public class ChangePasswordUserCommandValidator : AbstractValidator<ChangePasswordUserCommand>
{
    public ChangePasswordUserCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(5);
        RuleFor(x => x.ConfirmPassword).NotEmpty()
            .Equal(x => x.NewPassword)
            .WithMessage(ValidatorOptions.Global.LanguageManager.Culture?.Name.Contains("ru",
                StringComparison.OrdinalIgnoreCase) ?? false
                ? "Пароли не совпадают"
                : "The passwords don't match");
    }
}