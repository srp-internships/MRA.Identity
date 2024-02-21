using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Application.Common.Sieve;
using MRA.Identity.Application.Contract.Common;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Query;

public class GetAllUsersQueryHandler(
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IApplicationSieveProcessor sieveProcessor)
    : IRequestHandler<GetAllUsersQueryByFilters, PagedList<UserResponse>>
{
    public async Task<PagedList<UserResponse>> Handle(GetAllUsersQueryByFilters request, CancellationToken cancellationToken)
    {
        var users = userManager.Users
            .Include(u => u.UserSkills)
            .ThenInclude(s => s.Skill)
            .AsNoTracking();
        
        if (!request.Skills.IsNullOrEmpty())
        {
            var skills = request.Skills.Split(',').Select(s => s.Trim()).Distinct();
            users = users.Where(u =>
                skills.Intersect(u.UserSkills.Select(s => s.Skill.Name)).Count() == skills.Count());
        }

        var result = sieveProcessor.ApplyAdnGetPagedList(request,
            users, mapper.Map<UserResponse>);

        return await Task.FromResult(result);
    }
}