using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.UserRoles.Queries;
using MRA.Identity.Application.Contract.UserRoles.Response;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.UserRoles.Queries;

public class GetUserRolesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetUserRolesQuery, List<UserRolesResponse>>
{
    public async Task<List<UserRolesResponse>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        return await NewMethodForGetUserRole(request, cancellationToken);
    }

    private async Task<List<UserRolesResponse>> NewMethodForGetUserRole(GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<ApplicationUserRole> query = context.UserRoles;
        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            var user = await context.Users.FirstOrDefaultAsync(s =>
                    s.NormalizedUserName != null && s.NormalizedUserName.Contains(request.UserName!.ToUpper()),
                cancellationToken: cancellationToken);
            query = user != null
                ? query.Where(s => s.UserId == user.Id)
                : throw new NotFoundException(nameof(user), request.UserName);
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var role = await context.Roles.FirstOrDefaultAsync(s =>
                    s.NormalizedName != null && s.NormalizedName.Contains(request.Role!.ToUpper()),
                cancellationToken: cancellationToken);
            if (role != null)
            {
                query = query.Where(s => s.RoleId == role.Id);
            }
        }

        var raw = await query.ToArrayAsync(cancellationToken: cancellationToken);
        var res = new List<UserRolesResponse>();
        foreach (var userRole in raw)
        {
            res.Add(new UserRolesResponse
            {
                UserName = (await context.Users.FirstAsync(s => s.Id == userRole.UserId, cancellationToken: cancellationToken)).UserName!,
                RoleName = (await context.Roles.FirstAsync(s => s.Id == userRole.RoleId, cancellationToken: cancellationToken)).Name!,
                Slug = userRole.Slug
            });
        }

        return res;
    }
}