using MediatR;

namespace MRA.Identity.Application.Contract.EmailTemplates.Commands;

public class UpdateEmailTemplateCommand : IRequest<string>
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Subject { get; set; }
    public required string Text { get; set; }
}