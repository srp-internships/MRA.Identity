using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Query;

public class GetUserByKeyHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    : IRequestHandler<GetUserByKeyQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByKeyQuery request, CancellationToken cancellationToken)
    {
        ApplicationUser user;
        if (Guid.TryParse(request.Key, out var userId))
        {
            user=await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId,
                cancellationToken: cancellationToken);
        }
        else
        {
            user=await userManager.Users.FirstOrDefaultAsync(u => u.UserName == request.Key,
                cancellationToken: cancellationToken);
        }

        if (user == null)
            throw new NotFoundException("User not found");
        
        var result = mapper.Map<UserResponse>(user);
        return result;
    }
}