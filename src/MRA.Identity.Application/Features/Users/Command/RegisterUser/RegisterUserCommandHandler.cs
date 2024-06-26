﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.User.Commands.RegisterUser;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Command.RegisterUser;

public class RegisterUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IApplicationDbContext context,
    IEmailVerification emailVerification,
    ISmsCodeChecker codeChecker,
    IApplicationUserLinkService applicationUserLinkService)
    : IRequestHandler<RegisterUserCommand, Guid>
{
    public async Task<Guid> Handle(RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var exitingUser = await context.Users.FirstOrDefaultAsync(
            u => u.PhoneNumber == request.PhoneNumber || u.Email == request.Email,
            cancellationToken: cancellationToken);
        if (exitingUser != null)
        {
            if (exitingUser.Email == request.Email && exitingUser.PhoneNumber == request.PhoneNumber)
            {
                throw new ExistException(
                    $"Email {request.Email} and Phone Number {request.PhoneNumber} are not available!");
            }

            if (exitingUser.PhoneNumber == request.PhoneNumber)
            {
                throw new ExistException($"Phone Number {request.PhoneNumber} is not available!");
            }

            if (exitingUser.Email == request.Email)
            {
                throw new ExistException($"Email {request.Email} is not available!");
            }
        }

        ApplicationUser user = new()
        {
            Id = Guid.NewGuid(),
            UserName = request.Username.Trim(),
            NormalizedUserName = request.Username.Trim().ToLower(),
            Email = request.Email,
            NormalizedEmail = request.Email.ToLower(),
            EmailConfirmed = false,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = new DateTime(2000, 1, 1)
        };
        IdentityResult createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            throw new UnauthorizedAccessException(createResult.Errors.First().Description);
        }

        var application = await applicationUserLinkService.CreateUserLinkAsync(user.Id, request.ApplicationId,
            request.CallBackUrl, cancellationToken: cancellationToken);

        if (!application.IsProtected)
        {
            bool phoneVerified = codeChecker.VerifyPhone(request.VerificationCode, request.PhoneNumber);
            if (phoneVerified) user.PhoneNumberConfirmed = true;
            else
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync(cancellationToken);
                throw new ValidationException("Phone number is not verified");
            }
        }

        await emailVerification.SendVerificationEmailAsync(user);

        await context.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}