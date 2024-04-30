using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Domain.Entities;
using ClaimTypes = MRA.Configurations.Common.Constants.ClaimTypes;

namespace MRA.Identity.Infrastructure.Identity;

public class CustomUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IApplicationDbContext context,
    IOptions<IdentityOptions> options)
    : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>(userManager, roleManager, options)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var claims = await base.GenerateClaimsAsync(user);
        var userLinks = await context.ApplicationUserLinks.Where(s => s.UserId == user.Id).ToArrayAsync();
        claims.AddClaims(userLinks.Select(f => new Claim(ClaimTypes.ApplicationId, f.ApplicationId.ToString())));
        return claims;
    }
}