using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Query;

public class GetUserByKeyHandler(
    UserManager<ApplicationUser> userManager,
    IApplicationDbContext dbContext,
    IMapper mapper)
    : IRequestHandler<GetUserByKeyQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByKeyQuery request, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications.FirstOrDefaultAsync(x =>
                x.Id == request.ApplicationId && x.ClientSecret == request.ApplicationClientSecret,
            cancellationToken);

        if (application is null)
            throw new NotFoundException("There is no such application");
        
        ApplicationUser user;
        if (Guid.TryParse(request.Key, out var userId))
        {
            user = await userManager.Users
                .Include(x => x.ApplicationUserLinks)
                .FirstOrDefaultAsync(
                    u => u.Id == userId && u.ApplicationUserLinks.Any(l => l.ApplicationId == application.Id),
                    cancellationToken: cancellationToken);
        }
        else
        {
            user = await userManager.Users
                .Include(u => u.ApplicationUserLinks)
                .FirstOrDefaultAsync(
                    u => u.UserName == request.Key &&
                         u.ApplicationUserLinks.Any(l => l.ApplicationId == application.Id),
                    cancellationToken: cancellationToken);
        }

        if (user == null)
            throw new NotFoundException("User not found");

        var result = mapper.Map<UserResponse>(user);
        return result;
    }
}