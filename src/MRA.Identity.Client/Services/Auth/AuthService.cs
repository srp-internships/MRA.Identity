using AltairCA.Blazor.WebAssembly.Cookie;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Application.Contract.User.Commands.ChangePassword;
using MRA.Identity.Application.Contract.User.Commands.LoginUser;
using MRA.Identity.Application.Contract.User.Commands.RegisterUser;
using MRA.Identity.Application.Contract.User.Commands.ResetPassword;
using MRA.Identity.Application.Contract.User.Queries.CheckUserDetails;
using MRA.Identity.Application.Contract.User.Queries.GetUserNameByPhoneNymber;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Client.Services.ContentService;
using MRA.Identity.Client.Services.HttpClientService;
using MRA.Identity.Client.Services.Profile;
using System.Net;

namespace MRA.Identity.Client.Services.Auth;

public class AuthService(IHttpClientService httpClient,
        AuthenticationStateProvider authenticationStateProvider, NavigationManager navigationManager,
        IAltairCABlazorCookieUtil cookieUtil, IUserProfileService userProfileService, IConfiguration configuration, IContentService ContentService)
    : IAuthService
{
    public async Task<ApiResponse<bool>> ChangePassword(ChangePasswordUserCommand command)
    {

        var result = await httpClient.PutAsJsonAsync<bool>("Auth/ChangePassword", command);
        return result;
    }

    public async Task<ApiResponse<bool>> IsAvailableUserPhoneNumber(IsAvailableUserPhoneNumberQuery query)
    {
        var result = await httpClient.GetAsJsonAsync<bool>($"Auth/IsAvailableUserPhoneNumber/{Uri.EscapeDataString(query.PhoneNumber)}");
        return result;
    }

    public async Task<string> LoginUserAsync(LoginUserCommand command, bool newRegister = false)
    {
        string errorMessage = null;
        try
        {
            var result = await httpClient.PostAsJsonAsync<JwtTokenResponse>("Auth/login", command);
            if (result.Success)
            {
                string callbackUrl = string.Empty;
                string page = string.Empty;
                var currentUri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
                if (QueryHelpers.ParseQuery(currentUri.Query).TryGetValue("callback", out var param))
                    callbackUrl = param;
                if (QueryHelpers.ParseQuery(currentUri.Query).TryGetValue("page", out param))
                    page = param;
                if (callbackUrl.IsNullOrEmpty()) callbackUrl = configuration["HttpClient:JobsClient"];

                var response = result.Result;

                navigationManager.NavigateTo($"{callbackUrl}?atoken={response.AccessToken}&rtoken={response.RefreshToken}&vdate={response.AccessTokenValidateTo}&page={page}");

                await cookieUtil.SetValueAsync("authToken", response, secure:true);
                await authenticationStateProvider.GetAuthenticationStateAsync();
                if (!newRegister)
                    navigationManager.NavigateTo("/");
                return null;
            }

            if (result.HttpStatusCode == HttpStatusCode.Unauthorized)
            {
                errorMessage =result.Error;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex);
            errorMessage = ContentService["Profile:Servernotrespondingtry"];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            errorMessage = ContentService["Profile:Anerroroccurred"];
        }

        return errorMessage;
    }

    public async Task<string> RegisterUserAsync(RegisterUserCommand command)
    {
        try
        {
            command.PhoneNumber = command.PhoneNumber.Trim();
            if (command.PhoneNumber.Length == 9) command.PhoneNumber = "+992" + command.PhoneNumber.Trim();
            else if (command.PhoneNumber.Length == 12 && command.PhoneNumber[0] != '+') command.PhoneNumber = "+" + command.PhoneNumber;

            var result = await httpClient.PostAsJsonAsync<Guid>("Auth/register", command);
            if (result.Success)
            {
                await LoginUserAsync(new LoginUserCommand()
                {
                    Password = command.Password,
                    Username = command.Username
                });
                await userProfileService.Get();
                navigationManager.NavigateTo("/");

                return "";
            }
            if (result.HttpStatusCode is not (HttpStatusCode.Unauthorized or HttpStatusCode.BadRequest))
                return ContentService["Profile:Servernotrespondingtry"];

            var response = result.Error;
            return response;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex);
            return ContentService["Profile:Servernotrespondingtry"];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ContentService["Profile:Anerroroccurred"];
        }
    }

    public async Task<ApiResponse<bool>> ResetPassword(ResetPasswordCommand command)
    {
        var result = await httpClient.PostAsJsonAsync<bool>("Auth/ResetPassword", command);
        return result;
    }

    public async Task<ApiResponse> CheckUserName(string userName)
    {
        var result = await httpClient.GetAsJsonAsync<ApiResponse>($"User/CheckUserName/{userName}");
        return result;
    }

    public async Task<ApiResponse<UserDetailsResponse>> CheckUserDetails(CheckUserDetailsQuery query)
    {
        var result = await httpClient.GetAsJsonAsync<UserDetailsResponse>($"User/CheckUserDetails/{query.UserName}/{query.PhoneNumber}/{query.Email}");
        return result;
    }

    public async Task<ApiResponse> ResendVerificationEmail()
    {
        var result = await httpClient.PostAsJsonAsync<ApiResponse>("Auth/VerifyEmail", null);
        return result;
    }

    public async Task SendVerificationEmailToken(string token, string userId)
    {
        await httpClient.GetAsJsonAsync<ApiResponse>($"Auth/verify?token={WebUtility.UrlEncode(token)}&userid={userId}");
    }
}