using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.Skills.Command;
using MRA.Identity.Application.Contract.Skills.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Skills.Command;

public class AddSkillsCommandHandler(
    IApplicationDbContext context,
    IUserHttpContextAccessor userHttpContextAccessor)
    : IRequestHandler<AddSkillsCommand, UserSkillsResponse>
{
    public async Task<UserSkillsResponse> Handle(AddSkillsCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.UserName.Equals(userHttpContextAccessor.GetUserName()),
                cancellationToken);
        _ = user ?? throw new NotFoundException("user is not found");
       
        var userSkills = await context.UserSkills.Where(us => us.UserId == user.Id).ToListAsync(cancellationToken);

        foreach (var skillName in request.Skills)
        {
            var existingSkill = await context.Skills.FirstOrDefaultAsync(s => s.Name == skillName, cancellationToken);

            if (existingSkill == null)
            {
                existingSkill = new Skill { Name = skillName };
                context.Skills.Add(existingSkill);
            }

            if (userSkills.All(us => us.SkillId != existingSkill.Id))
            {
                userSkills.Add(new UserSkill { Skill = existingSkill });
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        var response = new UserSkillsResponse
        {
            Skills = userSkills.Select(us => us.Skill.Name).ToList()
        };

        return response;
    }
}