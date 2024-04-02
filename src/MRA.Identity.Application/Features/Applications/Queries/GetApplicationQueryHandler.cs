using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.Applications.Queries;
using MRA.Identity.Application.Contract.Applications.Responses;

namespace MRA.Identity.Application.Features.Applications.Queries;

public class GetApplicationQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetApplicationQuery, ApplicationResponse>
{
    public async Task<ApplicationResponse> Handle(GetApplicationQuery request, CancellationToken cancellationToken)
    {
        var application =
            await context.Applications.FirstOrDefaultAsync(s => s.Slug == request.Slug, cancellationToken) ??
            throw new NotFoundException($"application with slug '{request.Slug}' not found");
        return mapper.Map<ApplicationResponse>(application);
    }
}