using FluentValidation;
using MRA.Identity.Application.Contract.Applications.Commands;

namespace MRA.Identity.Application.Contract.Applications.Validators;

public class UpdateApplicationCommandValidator : AbstractValidator<UpdateApplicationCommand>
{
    public UpdateApplicationCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
        RuleFor(v => v.CallbackUrls)
            .Must(v => v.All(s => Uri.TryCreate(s, UriKind.Absolute, out _)))
            .WithMessage("Invalid callback url.");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result =
            await ValidateAsync(
                ValidationContext<UpdateApplicationCommand>.CreateWithOptions((UpdateApplicationCommand)model,
                    x => x.IncludeProperties(propertyName)));
        return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
    };
}