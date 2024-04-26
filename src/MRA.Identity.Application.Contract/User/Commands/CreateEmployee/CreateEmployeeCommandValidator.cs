using FluentValidation;
using MRA.Identity.Application.Contract.ContentService;

namespace MRA.Identity.Application.Contract.User.Commands.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator(IContentService contentService)
    {
        RuleFor(s => s.Email).EmailAddress();
        RuleFor(s => s.Username).NotEmpty();
        RuleFor(s => s.FirstName).NotEmpty();
        RuleFor(s => s.LastName).NotEmpty();
        RuleFor(s => s.PhoneNumber).NotEmpty()
            .Matches(@"^(?:\d{9}|\+992\d{9}|992\d{9})$")
            .WithMessage(contentService["PhoneNumberMessage"]);
        RuleFor(s => s.Password).NotEmpty().MinimumLength(5);
        RuleFor(s => s.Username).MinimumLength(5)
            .Must(username => username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '@'));
    }
}