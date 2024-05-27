using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MRA.Identity.Application.Contract.ContentService;

namespace MRA.Identity.Application.Contract.Messages.Commands;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator(IContentService contentService)
    {
        RuleFor(sm => sm.Text).NotEmpty();
        RuleFor(sm => sm.Phone).NotEmpty()
            .Matches(@"^(?:\d{9}|\+992\d{9}|992\d{9})$")
            .WithMessage(contentService["PhoneNumberMessage"]);
    }
}