using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.EmailTemplates.Queries;
using MRA.Identity.Application.Contract.EmailTemplates.Responses;

namespace MRA.Identity.Application.Features.EmailTemplates.Queries;

public class GetTemplateQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetEmailTemplateQuery, EmailTemplateResponse>
{
    public async Task<EmailTemplateResponse> Handle(GetEmailTemplateQuery request, CancellationToken cancellationToken)
    {
        return mapper.Map<EmailTemplateResponse>(
            await context.EmailTemplates.FirstOrDefaultAsync(s => s.Slug == request.Slug, cancellationToken)
            ?? throw new NotFoundException($"the template with slug {request.Slug} not found"));
    }
}