using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.EmailTemplates.Queries;
using MRA.Identity.Application.Contract.EmailTemplates.Responses;

namespace MRA.Identity.Application.Features.EmailTemplates.Queries;

public class GetSubjectsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetEmailTemplateNamesQuery, List<EmailTemplateNamesResponse>>
{
    public Task<List<EmailTemplateNamesResponse>> Handle(GetEmailTemplateNamesQuery request,
        CancellationToken cancellationToken)
    {
        return context.EmailTemplates.Select(s => new EmailTemplateNamesResponse
        {
            Slug = s.Slug,
            Name = s.Name
        }).ToListAsync(cancellationToken);
    }
}