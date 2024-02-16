using MediatR;

namespace MRA.Identity.Application.Contract.EmailTemplates.Commands;

public class CreateEmailTemplateCommand : IRequest<string>
{
    public required string Subject { get; set; }
    public required string Text { get; set; }
}