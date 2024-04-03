using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.Applications.Queries;
using MRA.Identity.Application.Contract.Applications.Responses;

namespace MRA.Identity.Application.Features.Applications.Queries;

public class GetApplicationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetApplicationsQuery, List<ApplicationResponse>>
{
    public async Task<List<ApplicationResponse>> Handle(GetApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var response = await context.Applications.Select(s => mapper.Map<ApplicationResponse>(s))
            .ToListAsync(cancellationToken);
        return response;
    }
}