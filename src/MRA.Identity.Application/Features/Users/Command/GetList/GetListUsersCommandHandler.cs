using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.User.Commands.UsersByApplications;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Command.GetList;

public class GetListUsersCommandHandler(
    UserManager<ApplicationUser> userManager,
    IApplicationDbContext dbContext,
    IMapper mapper) : IRequestHandler<GetListUsersCommand, List<UserResponse>>
{
    public async Task<List<UserResponse>> Handle(GetListUsersCommand request, CancellationToken cancellationToken)
    {
        var application =
            await dbContext.Applications.FirstOrDefaultAsync(a =>
                a.Id == request.ApplicationId, cancellationToken);
        if (application == null) throw new NotFoundException("Application not found");

        if (application.ClientSecret != request.ApplicationClientSecret)
            throw new ForbiddenAccessException("Invalid secret");

        var users = userManager.Users.Include(u => u.ApplicationUserLinks)
            .Where(u => u.ApplicationUserLinks.Any(l =>
                l.ApplicationId == application.Id));

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