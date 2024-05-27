using MediatR;
using MRA.Identity.Application.Contract.EmailTemplates.Responses;

namespace MRA.Identity.Application.Contract.EmailTemplates.Queries;

public class GetEmailTemplateQuery : IRequest<EmailTemplateResponse>
{
    public required string Slug { get; set; }
}