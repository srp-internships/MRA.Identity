namespace MRA.Identity.Application.Common.Interfaces.Services;

public interface IApplicationUserLinkService
{
    public Task<Domain.Entities.Application> CreateUserLinkAsync(Guid userId, Guid applicationId, string callback,
        bool? checkProtected = true,
        CancellationToken cancellationToken = default);

    public Task CreateUserLinkIfNotExistAsync(Guid userId, Guid applicationId,
        string callback, bool? checkProtected = true, CancellationToken cancellationToken = default);
}