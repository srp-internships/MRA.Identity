using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Sieve;
using MRA.Identity.Application.Contract.Common;
using MRA.Identity.Application.Contract.User.Commands.UsersByApplications;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Command.GetPagedList;

public class GetPagedListUsersCommandHandler(
    IApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IApplicationSieveProcessor sieveProcessor) : IRequestHandler<GetPagedListUsersCommand, PagedList<UserResponse>>
{
    public async Task<PagedList<UserResponse>> Handle(GetPagedListUsersCommand request,
        CancellationToken cancellationToken)
    {
        var application =
            await dbContext.Applications.FirstOrDefaultAsync(a =>
                a.Id == request.ApplicationId, cancellationToken);
        if (application == null) throw new NotFoundException("Application not found");

        if (application.ClientSecret != request.ApplicationClientSecret)
            throw new ForbiddenAccessException("Invalid secret");

        var users = userManager.Users.Include(u => u.ApplicationUserLinks)
            .Where(u => u.ApplicationUserLinks.Any(l => l.ApplicationId == application.Id));

        if (!request.Skills.IsNullOrEmpty())
        {
            users = users
                .Include(u => u.UserSkills)
                .ThenInclude(s => s.Skill)
                .AsNoTracking();

            var skills = request.Skills.Split(',').Select(s => s.Trim()).Distinct();
            users = users.Where(u =>
                skills.Intersect(u.UserSkills.Select(s => s.Skill.Name)).Count() == skills.Count());
        }

        var result = sieveProcessor.ApplyAdnGetPagedList(request,
            users, mapper.Map<UserResponse>);

        return result;
    }
}