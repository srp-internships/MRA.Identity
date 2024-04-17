using MediatR;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Application.Contract.User.Queries;

public class GetUserByKeyQuery : IRequest<UserResponse>
{
    public string Key { get; set; }
}