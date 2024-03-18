using FluentValidation;


namespace MRA.Identity.Application.Contract.User.Queries.GetUserNameByPhoneNumber;

public class GetUserNameByPhoneNumberQueryValidator : AbstractValidator<IsAvailableUserPhoneNumberQuery>
{
    public GetUserNameByPhoneNumberQueryValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(p => p.PhoneNumber).Matches(@"^\+992\d{9}$")
            .WithMessage(
                ValidatorOptions.Global.LanguageManager.Culture?.Name.Contains("ru",
                    StringComparison.OrdinalIgnoreCase) ?? false
                    ? "Неверный номер телефона. Пример : +992921234567"
                    : "Invalid phone number. Example : +992921234567");
    }
}