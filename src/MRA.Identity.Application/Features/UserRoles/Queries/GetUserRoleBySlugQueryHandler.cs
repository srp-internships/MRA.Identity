﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.UserRoles.Queries;
using MRA.Identity.Application.Contract.UserRoles.Response;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.UserRoles.Queries;
public class GetUserRoleBySlugQueryHandler : IRequestHandler<GetUserRolesBySlugQuery, UserRolesResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public GetUserRoleBySlugQueryHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext context, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _context = context;
        _roleManager = roleManager;
    }
    public async Task<UserRolesResponse> Handle(GetUserRolesBySlugQuery request, CancellationToken cancellationToken)
    {
        var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.Slug == request.Slug);
        _ = userRole ?? throw new NotFoundException("not found");
        var response = new UserRolesResponse
        {
            UserName = (await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userRole.UserId)).UserName,
            RoleName = (await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == userRole.RoleId)).Name,
            Slug = userRole.Slug
        };
        return response;
    }
}

