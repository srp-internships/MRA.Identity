using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.User.Commands.LoginUser;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Command.LoginUser;

public class LoginUserCommandHandler(
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IJwtTokenService jwtTokenService,
    IApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IApplicationUserLinkService applicationUserLinkService)
    : IRequestHandler<LoginUserCommand, JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        ApplicationUser user =
            await userManager.Users.FirstOrDefaultAsync(u => u.UserName == request.Username, cancellationToken) ??
            throw new UnauthorizedAccessException("Username is not found.");

        bool success = await userManager.CheckPasswordAsync(user, request.Password);

        if (!success)
            throw new UnauthorizedAccessException("Incorrect password.");

        //Check userLink if not superAdmin
        if (user.UserName != "SuperAdmin")
        {
            await applicationUserLinkService.CreateUserLinkIfNotExistAsync(user.Id, request.ApplicationId,
                request.CallBackUrl, cancellationToken: cancellationToken);
        }

        //var claims = (await userClaimsPrincipalFactory.CreateAsync(user)).Claims.ToList();
        var userClaims = await context.UserClaims
            .Where(uc => uc.UserId == user.Id)
            .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
            .ToListAsync(cancellationToken);

        var userRoles = await userManager.GetRolesAsync(user);
        var roleClaims = userRoles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        var applicationIds = await context.ApplicationUserLinks
            .Where(ul => ul.UserId == user.Id)
            .Select(ul => ul.ApplicationId)
            .ToListAsync(cancellationToken);

        var applicationClaims = applicationIds.Select(id => new Claim("applicationId", id.ToString())).ToList();

        var claims = userClaims.Concat(roleClaims).Concat(applicationClaims).ToList();

        return new JwtTokenResponse
        {
            RefreshToken = jwtTokenService.CreateRefreshToken(claims),
            AccessToken = jwtTokenService.CreateTokenByClaims(claims, out var expireDate),
            AccessTokenValidateTo = expireDate
        };
    }
}