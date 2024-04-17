using MediatR;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Application.Contract.User.Commands.UsersByApplications;

public class GetListUsersCommand : IRequest<List<UserResponse>>
{
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Skills { get; set; }
    public Guid? ApplicationId { get; set; }
    public string ApplicationClientSecret { get; set; }
}