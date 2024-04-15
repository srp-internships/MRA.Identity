using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.Profile.Queries;
using MRA.Identity.Application.Contract.Profile.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.UserProfiles.Query;

public class GetProfileQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IUserHttpContextAccessor userHttpContextAccessor)
    : IRequestHandler<GetProfileQuery, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var userName = request.UserName ?? userHttpContextAccessor.GetUserName();
        var userRoles = userHttpContextAccessor.GetUserRoles();

        if (request.UserName != null && !userRoles.Any())
            throw new ForbiddenAccessException("Access is denied");

        var user = GetUserQuery(context.Users, u => u.UserName == userName);

        if (userHttpContextAccessor.GetUserName() != "SuperAdmin" & request.UserName != null)
        {
            var application =
                await GetApplication(request.ApplicationId, request.ApplicationClientSecret, cancellationToken);
            user = GetUserQuery(user.Include(u => u.ApplicationUserLinks),
                u => u.ApplicationUserLinks.Any(l => l.ApplicationId == application.Id));
        }

        var _user = user.FirstOrDefault();
        if (_user == null) throw new NotFoundException("user is not found");
        var response = mapper.Map<UserProfileResponse>(_user);
        return response;
    }

    private IQueryable<ApplicationUser> GetUserQuery(IQueryable<ApplicationUser> users,
        Expression<Func<ApplicationUser, bool>> predicate)
    {
        return users.Where(predicate);
    }

    private async Task<Domain.Entities.Application> GetApplication(Guid applicationId, string applicationClientSecret,
        CancellationToken cancellationToken)
    {
        var application = await context.Applications.FirstOrDefaultAsync(x =>
                x.Id == applicationId && x.ClientSecret == applicationClientSecret,
            cancellationToken);

        if (application is null)
            throw new NotFoundException("There is no such application");

        return application;
    }
}