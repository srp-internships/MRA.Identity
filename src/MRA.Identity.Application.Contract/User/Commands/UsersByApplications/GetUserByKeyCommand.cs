using MediatR;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Application.Contract.User.Commands.UsersByApplications;

public class GetUserByKeyCommand :IRequest<UserResponse>
{
    public string Key { get; set; }
    public Guid? ApplicationId { get; set; }
    public string ApplicationClientSecret { get; set; }
}