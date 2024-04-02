using FluentValidation;
using MRA.Identity.Application.Contract.ContentService;

namespace MRA.Identity.Application.Contract.Profile.Commands.UpdateProfile;
public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator(IContentService contentService)
    {
        RuleFor(p => p.Email).EmailAddress();
        RuleFor(p => p.FirstName).NotEmpty();
        RuleFor(p => p.LastName).NotEmpty();
        RuleFor(sm => sm.PhoneNumber).NotEmpty()
            .Matches(@"^(?:\d{9}|\+992\d{9}|992\d{9})$")
            .WithMessage(contentService["PhoneNumberMessage"]);
        RuleFor(p => p.DateOfBirth).NotEmpty();
    }
}
