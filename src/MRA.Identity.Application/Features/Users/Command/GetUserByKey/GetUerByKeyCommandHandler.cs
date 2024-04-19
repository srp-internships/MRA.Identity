using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.User.Commands.UsersByApplications;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Command.GetUserByKey;

public class GetUerByKeyCommandHandler(
    UserManager<ApplicationUser> userManager,
    IApplicationDbContext dbContext,
    IMapper mapper) : IRequestHandler<GetUserByKeyCommand, UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByKeyCommand request, CancellationToken cancellationToken)
    {
        var isGuid = Guid.TryParse(request.Key, out var userId);

        var application =
            await dbContext.Applications.FirstOrDefaultAsync(a =>
                a.Id == request.ApplicationId, cancellationToken);
        if (application == null) throw new NotFoundException("Application not found");

        if (application.ClientSecret != request.ApplicationClientSecret)
            throw new ForbiddenAccessException("Invalid secret");

        var user = await userManager.Users.Include(u => u.ApplicationUserLinks)
            .FirstOrDefaultAsync(u => u.ApplicationUserLinks.Any(l => l.ApplicationId == application.Id) &&
                                      (isGuid ? u.Id == userId : u.UserName == request.Key), cancellationToken);

        if (user == null) throw new NotFoundException("User not found");

        return mapper.Map<UserResponse>(user);
    }
}