using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.Profile.Queries;
using MRA.Identity.Application.Contract.Profile.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.UserProfiles.Query;

public class GetProfileQueryHandler(
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IUserHttpContextAccessor userHttpContextAccessor)
    : IRequestHandler<GetProfileQuery, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var userName = userHttpContextAccessor.GetUserName();

        if (userName != "SuperAdmin")
        {
            if (!request.UserName.IsNullOrEmpty()) throw new ForbiddenAccessException("Access is denied");
            request.UserName = userName;
        }

        var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName,
            cancellationToken);

        if (user == null) throw new NotFoundException("user is not found");
        var response = mapper.Map<UserProfileResponse>(user);
        return response;
    }
}