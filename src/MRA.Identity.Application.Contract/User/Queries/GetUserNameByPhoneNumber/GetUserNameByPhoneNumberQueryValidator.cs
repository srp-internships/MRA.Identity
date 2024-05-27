using FluentValidation;
using MRA.Identity.Application.Contract.ContentService;


namespace MRA.Identity.Application.Contract.User.Queries.GetUserNameByPhoneNumber;

public class GetUserNameByPhoneNumberQueryValidator : AbstractValidator<IsAvailableUserPhoneNumberQuery>
{
    public GetUserNameByPhoneNumberQueryValidator(IContentService contentService)
    {
        RuleFor(sm => sm.PhoneNumber).NotEmpty()
            .Matches(@"^(?:\d{9}|\+992\d{9}|992\d{9})$")
            .WithMessage(contentService["PhoneNumberMessage"]);
    }
}