using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.EmailTemplates.Queries;
using MRA.Identity.Application.Contract.EmailTemplates.Responses;

namespace MRA.Identity.Application.Features.EmailTemplates.Queries;

public class GetSubjectsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetEmailTemplateSubjectsQuery, List<EmailTemplateSubjectResponse>>
{
    public Task<List<EmailTemplateSubjectResponse>> Handle(GetEmailTemplateSubjectsQuery request,
        CancellationToken cancellationToken)
    {
        return context.EmailTemplates.Select(s => new EmailTemplateSubjectResponse
        {
            Slug = s.Slug,
            Subject = s.Subject
        }).ToListAsync(cancellationToken);
    }
}