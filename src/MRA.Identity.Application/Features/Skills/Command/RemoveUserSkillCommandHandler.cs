using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.Skills.Command;
using MRA.Identity.Application.Contract.Skills.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Skills.Command;

public class RemoveUserSkillCommandHandler(
    IApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IUserHttpContextAccessor userHttpContextAccessor)
    : IRequestHandler<RemoveUserSkillCommand, UserSkillsResponse>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<UserSkillsResponse> Handle(RemoveUserSkillCommand request, CancellationToken cancellationToken)
    {
        var userName = userHttpContextAccessor.GetUserName();
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.UserName.Equals(userHttpContextAccessor.GetUserName()),
                cancellationToken);
        _ = user ?? throw new NotFoundException("user is not found");

        var userSkills = await context.UserSkills.Include(u => u.Skill).Where(us => us.UserId == user.Id)
            .ToListAsync(cancellationToken);


        var specificSkill = userSkills
            .FirstOrDefault(us => us.Skill.Name == request.Skill);
        _ = specificSkill ?? throw new NotFoundException($"Skill '{request.Skill}' not found for this user");

        userSkills.Remove(specificSkill);

        await context.SaveChangesAsync(cancellationToken);

        var response = new UserSkillsResponse
        {
            Skills = userSkills.Select(us => us.Skill.Name).ToList()
        };

        return response;
    }
}