using Microsoft.AspNetCore.Http;
using MRA.Configurations.Common.Constants;
using MRA.Identity.Application.Common.Interfaces.Services;

namespace MRA.Identity.Infrastructure.Account.Services;

public class UserHttpContextAccessor(IHttpContextAccessor httpContextAccessor) : IUserHttpContextAccessor
{
    public Guid GetUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;

        var idClaim = user?.FindFirst(ClaimTypes.Id);

        if (idClaim != null && Guid.TryParse(idClaim.Value, out Guid id))
            return id;

        return Guid.Empty;
    }

    public string GetUserName()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userNameClaim = user?.FindFirst(ClaimTypes.Username);

        return userNameClaim != null ? userNameClaim.Value : string.Empty;
    }

    public List<string> GetUserRoles()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var roleClaims = user?.FindAll(ClaimTypes.Role);

        return roleClaims?.Select(rc => rc.Value).ToList() ?? new List<string>();
    }

    public List<Guid> GetApplicationsIDs()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var applications = user?.FindAll(ClaimTypes.Application);

        return applications?.Where(rc => Guid.TryParse(rc.Value, out _))
            .Select(rc => Guid.Parse(rc.Value))
            .ToList() ?? new List<Guid>();
    }

}