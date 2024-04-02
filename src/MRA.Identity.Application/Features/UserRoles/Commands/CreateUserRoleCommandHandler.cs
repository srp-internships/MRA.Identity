using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.UserRoles.Commands;
using MRA.Identity.Domain.Entities;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.Services;

namespace MRA.Identity.Application.Features.UserRoles.Commands;

public class CreateUserRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IApplicationDbContext applicationDbContext,
    ISlugService slugService)
    : IRequestHandler<CreateUserRolesCommand, string>
{
    public async Task<string> Handle(CreateUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == request.UserName.ToUpper(),
            cancellationToken: cancellationToken) ?? throw new NotFoundException("user is not found");
        if (await userManager.IsInRoleAsync(user, request.RoleName))
            throw new ValidationException("user is already in that role");


        var role = await roleManager.FindByNameAsync(request.RoleName) ??
                   throw new NotFoundException("role is not found");

        var userRole = new ApplicationUserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            Slug = slugService.GenerateSlug(request.UserName + "-" + request.RoleName)
        };
        await applicationDbContext.UserRoles.AddAsync(userRole, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return userRole.Slug;
    }
}