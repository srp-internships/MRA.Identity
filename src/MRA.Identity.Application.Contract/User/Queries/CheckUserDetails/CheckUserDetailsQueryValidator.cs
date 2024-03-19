using FluentValidation;
using MRA.Identity.Application.Contract.ContentService;

namespace MRA.Identity.Application.Contract.User.Queries.CheckUserDetails;

public class CheckUserDetailsQueryValidator : AbstractValidator<CheckUserDetailsQuery>
{
    public CheckUserDetailsQueryValidator(IContentService contentService)
    {
        RuleFor(sm => sm.PhoneNumber).NotEmpty()
            .Matches(@"^(?:\d{9}|\+992\d{9}|992\d{9})$")
            .WithMessage(contentService["PhoneNumberMessage"]);
        RuleFor(s => s.UserName.Length > 3);
        RuleFor(s => s.Email).EmailAddress();
    }
}