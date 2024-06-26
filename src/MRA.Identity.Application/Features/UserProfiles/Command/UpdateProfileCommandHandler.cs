﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.Profile.Commands.UpdateProfile;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.UserProfiles.Command;

public class UpdateProfileCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUserHttpContextAccessor userHttpContextAccessor,
    IMapper mapper)
    : IRequestHandler<UpdateProfileCommand, bool>
{
    public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(userHttpContextAccessor.GetUserName());
        _ = user ?? throw new NotFoundException("user is not found");

        var exitingUser = await userManager.Users.FirstOrDefaultAsync(u => u.UserName != user.UserName &&
        (u.PhoneNumber == request.PhoneNumber || u.Email == request.Email), cancellationToken: cancellationToken);
        if (exitingUser != null)
        {
            if (exitingUser.Email == request.Email && exitingUser.PhoneNumber == request.PhoneNumber)
            {
                throw new DuplicateWaitObjectException($"Email {request.Email} and Phone Number {request.PhoneNumber} are not available!");
            }

            if (exitingUser.PhoneNumber == request.PhoneNumber)
            {
                throw new DuplicateWaitObjectException($"Phone Number {request.PhoneNumber} is not available!");
            }

            if (exitingUser.Email == request.Email)
            {
                throw new DuplicateWaitObjectException($"Email {request.Email} is not available!");
            }
        }

        if (user.Email != request.Email)
            user.EmailConfirmed = false;
        if (user.PhoneNumber != request.PhoneNumber) user.PhoneNumberConfirmed = false;
        mapper.Map(request, user);


        return (await userManager.UpdateAsync(user)).Succeeded;

    }
}
