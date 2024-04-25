using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.User.Commands.CreateEmployee;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.Users.Command.CreateEmployee;

public class CreateEmployeeCommandHandler(
    UserManager<ApplicationUser> userManager,
    IApplicationDbContext context,
    IUserHttpContextAccessor userHttpContextAccessor) : IRequestHandler<CreateEmployeeCommand, Guid>
{
    public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var applicationIds = userHttpContextAccessor.GetApplicationsIDs();
        if (!applicationIds.Any()) throw new NotFoundException("You haven't any applications!'");

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


        var applicationId = applicationIds.First();
        await context.ApplicationUserLinks.AddAsync(
            new ApplicationUserLink() { ApplicationId = applicationId, UserId = user.Id }, cancellationToken);


        await context.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}