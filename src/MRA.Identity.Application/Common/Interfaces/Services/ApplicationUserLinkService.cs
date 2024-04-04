namespace MRA.Identity.Application.Common.Interfaces.Services;

public interface IApplicationUserLinkService
{
    public Task CreateUserLinkAsync(Guid userId, Guid applicationId, string callback,
        CancellationToken cancellationToken);
}