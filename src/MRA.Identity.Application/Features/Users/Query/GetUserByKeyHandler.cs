using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Query;

public class GetUserByKeyHandler(
    UserManager<ApplicationUser> userManager,
    IApplicationDbContext dbContext,
    IMapper mapper,
    IUserHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetUserByKeyQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByKeyQuery request, CancellationToken cancellationToken)
    {
        ApplicationUser user;
        var isSuperAdmin = httpContextAccessor.GetUserName() == "SuperAdmin";
        var isGuid = Guid.TryParse(request.Key, out var userId);

        if (isSuperAdmin)
            user = await GetUserByCondition(u => isGuid ? u.Id == userId : u.UserName == request.Key,
                cancellationToken);
        else
        {
            var application =
                await GetApplication(request.ApplicationId, request.ApplicationClientSecret, cancellationToken);

            user = await GetUserByCondition(u => (isGuid ? u.Id == userId : u.UserName == request.Key) &&
                                                 u.ApplicationUserLinks.Any(l => l.ApplicationId == application.Id),
                cancellationToken,
                true);
        }

        if (user == null)
            throw new NotFoundException("User not found");

        var result = mapper.Map<UserResponse>(user);
        return result;
    }

    private async Task<ApplicationUser> GetUserByCondition(Expression<Func<ApplicationUser, bool>> condition,
        CancellationToken cancellationToken, bool includeLinks = false)
    {
        var users = userManager.Users;
        if (includeLinks)
            users = users.Include(u => u.ApplicationUserLinks);

        return await users.FirstOrDefaultAsync(condition, cancellationToken);
    }

    private async Task<Domain.Entities.Application> GetApplication(Guid applicationId, string applicationClientSecret,
        CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications.FirstOrDefaultAsync(x =>
                x.Id == applicationId && x.ClientSecret == applicationClientSecret,
            cancellationToken);

        if (application is null)
            throw new NotFoundException("There is no such application");

        return application;
    }
}