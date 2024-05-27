using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.Claim.Commands;

namespace MRA.Identity.Application.Features.Claims.Commands;

public class UpdateClaimCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateClaimCommand, Unit>
{
    public async Task<Unit> Handle(UpdateClaimCommand request, CancellationToken cancellationToken)
    {
        var claim = await context.UserClaims.SingleOrDefaultAsync(s => s.Slug == request.Slug.Trim().ToLower(), cancellationToken: cancellationToken);
        _ = claim ?? throw new NotFoundException($"claim with slug {request.Slug} not found");

        claim.ClaimValue = request.ClaimValue;

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }
}