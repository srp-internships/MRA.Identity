using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Query;

public class GetListUsersQueryHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<GetListUsersQuery, List<UserResponse>>
{
    public Task<List<UserResponse>> Handle(GetListUsersQuery request, CancellationToken cancellationToken)
    {
        var users = userManager.Users;
        if (!request.FullName.IsNullOrEmpty())
            users = users.Where(u => string.Concat(u.FirstName, ' ', u.LastName).Contains(request.FullName.Trim()));
        if (!request.Email.IsNullOrEmpty())
            users = users.Where(u => u.Email.Contains(request.Email.Trim()));
        if (!request.PhoneNumber.IsNullOrEmpty())
            users = users.Where(u => u.PhoneNumber.Contains(request.PhoneNumber.Trim()));

        if (!request.Skills.IsNullOrEmpty())
        {
            users.Include(u => u.UserSkills).ThenInclude(s => s.Skill);
            
        }

    }
}