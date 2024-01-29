using AltairCA.Blazor.WebAssembly.Cookie;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Client.Services.HttpClients;

namespace MRA.Identity.Client.Services.Auth
{
    public class TokenParserService(IAltairCABlazorCookieUtil cookieUtil, IHttpClientService httpClientService) : ITokenParserService
    {
        public async Task<JwtTokenResponse> GetTokenAsync()
        {
            var token = await cookieUtil.GetValueAsync<JwtTokenResponse>("authToken");
            if (token == null)
            {
                return null;
            }

            if (token.AccessTokenValidateTo <= DateTime.Now)
            {
                var refreshResponse = await httpClientService.PostAsJsonAsync<JwtTokenResponse>("auth/refresh",
                    new GetAccessTokenUsingRefreshTokenQuery
                    {
                        RefreshToken = token.RefreshToken,
                        AccessToken = token.AccessToken
                    });

                if (!refreshResponse.Success)
                    return null;

                token = refreshResponse.Result;
                await cookieUtil.SetValueAsync("authToken", token, secure: true);
            }

            return token;
        }
    }
}
