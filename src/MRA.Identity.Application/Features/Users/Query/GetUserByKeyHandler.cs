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
        var isGuid = Guid.TryParse(request.Key, out var userId);
        var isSuperAdmin = httpContextAccessor.GetUserName() == "SuperAdmin";
        var applications = isSuperAdmin ? null : httpContextAccessor.GetApplicationsIDs();

        var user = await userManager.Users
            .Include(u => isSuperAdmin ? null : u.ApplicationUserLinks)
            .FirstOrDefaultAsync(u =>
                    (isGuid ? u.Id == userId : u.UserName == request.Key) &&
                    (isSuperAdmin || u.ApplicationUserLinks.Any(l => applications.Contains(l.ApplicationId))),
                cancellationToken);

        if (user == null) throw new NotFoundException("User not found");

        return mapper.Map<UserResponse>(user);
    }
}