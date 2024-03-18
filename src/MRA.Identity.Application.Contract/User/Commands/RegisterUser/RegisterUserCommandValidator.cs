using FluentValidation;

namespace MRA.Identity.Application.Contract.User.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(s => s.Email).EmailAddress();
            RuleFor(s => s.Username).NotEmpty();
            RuleFor(s => s.FirstName).NotEmpty();
            RuleFor(s => s.LastName).NotEmpty();
            RuleFor(s => s.PhoneNumber).NotEmpty()
                .Matches(@"^(?:\d{9}|\+992\d{9}|992\d{9})$")
                .WithMessage(
                    ValidatorOptions.Global.LanguageManager.Culture?.Name.Contains("ru",
                        StringComparison.OrdinalIgnoreCase) ?? false
                        ? "Неверный номер телефона. Пример : +992921234567, 992921234567, 921234567"
                        : "Invalid phone number. Example : +992921234567, 992921234567, 921234567");

            RuleFor(s => s.Username).MinimumLength(5)
                .Must(username => username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '@'));

            RuleFor(s => s.Password).NotEmpty().MinimumLength(5);
            RuleFor(s => s.ConfirmPassword).Equal(s => s.Password)
                .WithMessage(
                    ValidatorOptions.Global.LanguageManager.Culture?.Name.Contains("ru",
                        StringComparison.OrdinalIgnoreCase) ?? false
                        ? "Пароли не совпадают"
                        : "The passwords don't match");
        }
    }
}