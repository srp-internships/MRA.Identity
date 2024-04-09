using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.User.Commands.LoginUser;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Command.LoginUser;

public class LoginUserCommandHandler(
    IJwtTokenService jwtTokenService,
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

        await applicationUserLinkService.CreateUserLinkIfNotExistAsync(user.Id, request.ApplicationId,
            request.CallBackUrl, cancellationToken: cancellationToken);

        var claims = await userManager.GetClaimsAsync(user);

        return new JwtTokenResponse
        {
            RefreshToken = jwtTokenService.CreateRefreshToken(claims),
            AccessToken = jwtTokenService.CreateTokenByClaims(claims, out var expireDate),
            AccessTokenValidateTo = expireDate
        };
    }
}