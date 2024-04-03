using MediatR;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Application.Contract.User.Queries;

public class GetListUsersQuery : IRequest<List<UserResponse>>
{
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Skills { get; set; }
    public Guid? ApplicationId { get; set; }
    public string ApplicationClientSecret { get; set; }
}