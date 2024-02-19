using MediatR;

namespace MRA.Identity.Application.Contract.EmailTemplates.Commands;
#nullable enable
public class UpdateEmailTemplateCommand : IRequest<string>
{
    public required string Slug { get; set; }
    public string? Subject { get; set; }
    public string? Text { get; set; }
}