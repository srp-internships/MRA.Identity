using FluentValidation;

namespace MRA.Identity.Application.Contract.User.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(s => s.Password).NotEmpty().MinimumLength(5);
        RuleFor(s => s.PhoneNumber).Matches(@"^\+992\d{9}$")
            .WithMessage(
                ValidatorOptions.Global.LanguageManager.Culture?.Name.Contains("ru",
                    StringComparison.OrdinalIgnoreCase) ?? false
                    ? "Неверный номер телефона. Пример : +992921234567"
                    : "Invalid phone number. Example : +992921234567");
        RuleFor(s => s.ConfirmPassword).Equal(s => s.Password)
             .WithMessage(
                ValidatorOptions.Global.LanguageManager.Culture?.Name.Contains("ru",
                    StringComparison.OrdinalIgnoreCase) ?? false
                    ? "Пароли не совпадают"
                    : "The passwords don't match");
    }
}