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
}