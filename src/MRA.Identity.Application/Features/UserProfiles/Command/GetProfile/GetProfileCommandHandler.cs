using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.Profile.Commands.GetProfile;
using MRA.Identity.Application.Contract.Profile.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.UserProfiles.Command.GetProfile;

public class GetProfileCommandHandler(
    IApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    IMapper mapper)
    : IRequestHandler<GetProfileCommand, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(GetProfileCommand request, CancellationToken cancellationToken)
    {
        var application =
            await dbContext.Applications.FirstOrDefaultAsync(a =>
                a.Id == request.ApplicationId, cancellationToken);
        if (application == null) throw new NotFoundException("Application not found");

        if (application.ClientSecret != request.ApplicationClientSecret)
            throw new ForbiddenAccessException("Invalid secret");

        var user = await userManager.Users.Include(u => u.ApplicationUserLinks)
            .FirstOrDefaultAsync(x =>
                x.UserName == request.UserName && x.ApplicationUserLinks.Any(l => l.ApplicationId == application.Id),
        cancellationToken);

        if (user == null) throw new NotFoundException("user is not found");
        var response = mapper.Map<UserProfileResponse>(user);
        return response;
    }
}