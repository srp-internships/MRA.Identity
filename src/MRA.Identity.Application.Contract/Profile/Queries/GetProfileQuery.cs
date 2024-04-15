using MediatR;
using MRA.Identity.Application.Contract.Profile.Responses;

namespace MRA.Identity.Application.Contract.Profile.Queries;

public class GetProfileQuery : IRequest<UserProfileResponse>
{
    public string UserName { get; set; }
    public Guid ApplicationId { get; set; }
    public string ApplicationClientSecret { get; set; }
}