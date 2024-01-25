using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Query;
public class GetUserBySlugHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    : IRequestHandler<GetUserByUsernameQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken: cancellationToken);
        var result = mapper.Map<UserResponse>(user);

        return result;
    }
}


public class GetUserByIdCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    : IRequestHandler<GetUserByUserIdQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken: cancellationToken);
        var result = mapper.Map<UserResponse>(user);

        return result;
    }
}