using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Query;

public class GetListUsersQueryHandler(
    UserManager<ApplicationUser> userManager,
    IApplicationDbContext dbContext,
    IMapper mapper)
    : IRequestHandler<GetListUsersQuery, List<UserResponse>>
{
    public async Task<List<UserResponse>> Handle(GetListUsersQuery request, CancellationToken cancellationToken)
    {
        var users = userManager.Users.AsQueryable();
        if (request.ApplicationId != null && !request.ApplicationClientSecret.IsNullOrEmpty())
        {
            if (await dbContext.Applications.FirstOrDefaultAsync(x =>
                        x.Id == request.ApplicationId && x.ClientSecret == request.ApplicationClientSecret,
                    cancellationToken) == null)
                throw new NotFoundException("There is no such application");

            users = users.Include(u => u.ApplicationUserLinks)
                .Where(u => u.ApplicationUserLinks.Any(l => l.ApplicationId == request.ApplicationId));
        }

        if (!request.FullName.IsNullOrEmpty())
            users = users.Where(u => (u.FirstName + " " + u.LastName).Contains(request.FullName.Trim()));
        if (!request.Email.IsNullOrEmpty())
            users = users.Where(u => u.Email.Contains(request.Email.Trim()));
        if (!request.PhoneNumber.IsNullOrEmpty())
            users = users.Where(u => u.PhoneNumber.Contains(request.PhoneNumber.Trim()));

        if (!request.Skills.IsNullOrEmpty())
        {
            users = users.Include(u => u.UserSkills).ThenInclude(s => s.Skill);
            var skills = request.Skills.Split(',').Select(s => s.Trim()).Distinct();
            users = users.Where(u =>
                skills.Intersect(u.UserSkills.Select(s => s.Skill.Name)).Count() == skills.Count());
        }

        return await Task.FromResult(mapper.Map<List<UserResponse>>(users.ToList()));
    }
}