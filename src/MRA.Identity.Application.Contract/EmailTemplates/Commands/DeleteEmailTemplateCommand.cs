using MediatR;

namespace MRA.Identity.Application.Contract.EmailTemplates.Commands;

public class DeleteEmailTemplateCommand : IRequest<Unit>
{
    public required string Slug { get; set; }
}