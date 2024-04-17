using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Common.Sieve;
using MRA.Identity.Application.Contract.Common;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Query;

public class GetPagedListUsersQueryHandler(
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IUserHttpContextAccessor userHttpContextAccessor,
    IApplicationSieveProcessor sieveProcessor)
    : IRequestHandler<GetPagedListUsersQuery, PagedList<UserResponse>>
{
    public Task<PagedList<UserResponse>> Handle(GetPagedListUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = userManager.Users
            .Include(u => u.UserSkills)
            .ThenInclude(s => s.Skill)
            .AsNoTracking();

        if (userHttpContextAccessor.GetUserName() != "SuperAdmin")
        {
            users = users.Include(u => u.ApplicationUserLinks)
                .Where(u => u.ApplicationUserLinks.Any(l =>
                    userHttpContextAccessor.GetApplicationsIDs().Contains(l.ApplicationId)));
        }

        if (!request.Skills.IsNullOrEmpty())
        {
            var skills = request.Skills.Split(',').Select(s => s.Trim()).Distinct();
            users = users.Where(u =>
                skills.Intersect(u.UserSkills.Select(s => s.Skill.Name)).Count() == skills.Count());
        }

        var result = sieveProcessor.ApplyAdnGetPagedList(request,
            users, mapper.Map<UserResponse>);

        return Task.FromResult(result);
    }
}