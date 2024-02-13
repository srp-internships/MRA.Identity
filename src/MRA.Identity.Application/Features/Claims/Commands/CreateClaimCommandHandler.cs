using MediatR;
using Microsoft.AspNetCore.Identity;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.Claim.Commands;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Claims.Commands;

public class CreateClaimCommandHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
    : IRequestHandler<CreateClaimCommand, Guid>
{
    public async Task<Guid> Handle(CreateClaimCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        _ = user ?? throw new ValidationException("user is not found");
        var claim = new ApplicationUserClaim
        {
            UserId = user.Id,
            ClaimType = request.ClaimType,
            ClaimValue = request.ClaimValue,
            Slug = user.UserName + "-" + request.ClaimValue
        };
        await context.UserClaims.AddAsync(claim, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return user.Id;

    }
}