using MediatR;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Application.Contract.User.Queries;
public class GetUserByUsernameQuery : IRequest<UserResponse>
{
    public string UserName { get; set; }
}

public class GetUserByUserIdQuery : IRequest<UserResponse>
{
    public Guid Id { get; set; }
}
