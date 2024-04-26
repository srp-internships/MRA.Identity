using MediatR;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Application.Contract.User.Commands.LoginUser;

public class LoginUserCommand : IRequest<JwtTokenResponse>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public Guid ApplicationId { get; set; }
    public string CallBackUrl { get; set; }
}