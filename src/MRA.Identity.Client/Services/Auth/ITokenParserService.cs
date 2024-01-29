using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Client.Services.Auth
{
    public interface ITokenParserService
    {
        Task<JwtTokenResponse> GetTokenAsync();
    }
}
