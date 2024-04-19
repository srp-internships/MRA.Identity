using FluentValidation;
using MediatR;
using MRA.Identity.Application.Contract.Profile.Responses;

namespace MRA.Identity.Application.Contract.Profile.Commands.GetProfile;

public class GetProfileCommand :IRequest<UserProfileResponse>
{
    public string UserName { get; set; }
    public Guid ApplicationId { get; set; }
    public string ApplicationClientSecret { get; set; }
}

public class GetProfileCommandValidator : AbstractValidator<GetProfileCommand>
{
    public GetProfileCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.ApplicationClientSecret).NotEmpty();
        RuleFor(x => x.ApplicationId).NotEmpty();
    }
}