using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Services;

public class ApplicationUserLinkService(IApplicationDbContext context) : IApplicationUserLinkService
{
    private Guid _userId;
    private Guid _applicationId;
    private string _callback;
    private MRA.Identity.Domain.Entities.Application _application;

    public async Task CreateUserLinkIfNotExistAsync(Guid userId, Guid applicationId,
        string callback, CancellationToken cancellationToken)
    {
        if (!await HasUserLink(cancellationToken))
            await CreateUserLinkAsync(userId, applicationId, callback, cancellationToken);
    }

    public async Task<Domain.Entities.Application> CreateUserLinkAsync(Guid userId, Guid applicationId, string callback,
        CancellationToken cancellationToken)
    {
        _userId = userId;
        _applicationId = applicationId;
        _callback = callback;
        _application =
            await context.Applications.FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken)
            ?? throw new NotFoundException($"application with id {applicationId} does not exist");
        CheckCallback();

        await AddApplicationUserLinkAsync(cancellationToken);

        var userRole = new ApplicationUserRole
        {
            UserId = userId,
            RoleId = _application.DefaultRoleId
        };
        
        if (!await context.UserRoles.AnyAsync(s => s.UserId == userRole.UserId && s.RoleId == userRole.RoleId,
                cancellationToken: cancellationToken))
            await context.UserRoles.AddAsync(userRole, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        return _application;
    }

    private async Task<bool> HasUserLink(CancellationToken cancellationToken) =>
        await context.ApplicationUserLinks.AnyAsync(
            s => s.ApplicationId == _applicationId && s.UserId == _userId, cancellationToken);

    private async Task AddApplicationUserLinkAsync(CancellationToken cancellationToken)
    {
        var applicationUserLink = new ApplicationUserLink
        {
            ApplicationId = _applicationId,
            UserId = _userId
        };
        await context.ApplicationUserLinks.AddAsync(applicationUserLink, cancellationToken);
    }

    private void CheckCallback()
    {
        if (!_application.CallbackUrls.Any(s => AreUrlsEqual(s, _callback)))
            throw new ValidationException("Callback is invalid");

        return;

        bool AreUrlsEqual(string url1, string url2)
        {
            url1 = url1.Trim().TrimEnd('/').ToLower();
            url2 = url2.Trim().TrimEnd('/').ToLower();
            return url1 == url2;
        }
    }
}