using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.Profile.Queries;
using MRA.Identity.Application.Contract.Profile.Responses;

namespace MRA.Identity.Application.Features.UserProfiles.Query;

public class GetProfileQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IUserHttpContextAccessor userHttpContextAccessor)
    : IRequestHandler<GetProfileQuery, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var userRoles = userHttpContextAccessor.GetUserRoles();
        var userName = userHttpContextAccessor.GetUserName();

        if (request.UserName != null)
            userName = request.UserName;

        var user = context.Users
            .Where(u => u.UserName == userName);


        if (request.UserName != null && !userRoles.Any())
            throw new ForbiddenAccessException("Access is denied");
        
        var application = await context.Applications.FirstOrDefaultAsync(x =>
                x.Id == request.ApplicationId && x.ClientSecret == request.ApplicationClientSecret,
            cancellationToken);

        if (!userRoles.Any(r => r == "SuperAdmin") && request.UserName != null)
        {
            if (application is null)
                throw new NotFoundException("There is no such application");

            user = user.Include(u => u.ApplicationUserLinks)
                .Where(u => u.ApplicationUserLinks.Any(l => l.ApplicationId == application.Id));
        }

        var _user = user.FirstOrDefault();
        if (_user == null) throw new NotFoundException("user is not found");
        var response = mapper.Map<UserProfileResponse>(_user);
        return response;
    }
}