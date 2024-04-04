using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Services;

public class ApplicationUserLinkService(IApplicationDbContext context):IApplicationUserLinkService
{
    public async Task CreateUserLinkAsync(Guid userId, Guid applicationId, string callback,
        CancellationToken cancellationToken)
    {
        var application =
            await context.Applications.FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken)
            ?? throw new NotFoundException($"application with id {applicationId} does not exist");
        if (!application.CallbackUrls.Any(s => AreUrlsEqual(s, callback)))
        {
            throw new ValidationException("Callback is invalid");
        }

        var applicationUserLink = new ApplicationUserLink
        {
            ApplicationId = application.Id,
            UserId = userId,
        };
        await context.ApplicationUserLinks.AddAsync(applicationUserLink, cancellationToken);

        var userRole = new ApplicationUserRole
        {
            UserId = userId,
            RoleId = application.DefaultRoleId,
        };
        await context.UserRoles.AddAsync(userRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private bool AreUrlsEqual(string url1, string url2)
    {
        url1 = url1.Trim().TrimEnd('/').ToLower();
        url2 = url2.Trim().TrimEnd('/').ToLower();
        return url1 == url2;
    }
}