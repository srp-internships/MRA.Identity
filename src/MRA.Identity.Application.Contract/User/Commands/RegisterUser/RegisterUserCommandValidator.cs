using FluentValidation;

namespace MRA.Identity.Application.Contract.User.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(s => s.Email).EmailAddress();
        RuleFor(s => s.Username).NotEmpty();
        RuleFor(s => s.FirstName).NotEmpty();
        RuleFor(s => s.LastName).NotEmpty();
        RuleFor(s => s.PhoneNumber).NotEmpty().Matches(@"^(?:\d{9}|\+992\d{9}|992\d{9})$");
        RuleFor(s => s.Username.Trim());
        RuleFor(s => s.Username).MinimumLength(5)
            .Must(username => username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '@'));

        RuleFor(s => s.Password).NotEmpty().MinimumLength(5);
        RuleFor(s => s.ConfirmPassword).Equal(s => s.Password);
    }
}