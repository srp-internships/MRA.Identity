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
using MRA.Identity.Client.Services.Profile;
using System.Net;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.BlazorComponents.Snackbar.Extensions;
using MudBlazor;
using MRA.BlazorComponents.HttpClient.Responses;

namespace MRA.Identity.Client.Services.Auth;

public class AuthService(
    IHttpClientService httpClient,
    NavigationManager navigationManager,
    IAltairCABlazorCookieUtil cookieUtil,
    IUserProfileService userProfileService,
    IConfiguration configuration,
    IContentService contentService,
    ISnackbar snackbar)
    : IAuthService
{
    public async Task<bool> ChangePassword(ChangePasswordUserCommand command)
    {
        var result =
            await httpClient.PutAsJsonAsync(configuration.GetIdentityUrl("Auth/ChangePassword"), command);
        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"]);
        return result.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> IsAvailableUserPhoneNumber(IsAvailableUserPhoneNumberQuery query)
    {
        var result =
            await httpClient.GetFromJsonAsync<bool>(
                configuration.GetIdentityUrl(
                    $"Auth/IsAvailableUserPhoneNumber/{Uri.EscapeDataString(query.PhoneNumber)}"));
        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"],
            contentService["Profile:Educationdetailsadded"]);
        return result.Result;
    }

    public async Task<bool> LoginUserAsync(LoginUserCommand command)
    {
        var result =
            await httpClient.PostAsJsonAsync<JwtTokenResponse>(configuration.GetIdentityUrl("Auth/login"), command);
        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"]);

        if (result.HttpStatusCode == HttpStatusCode.Unauthorized)
        {
            snackbar.Add(contentService["SignIn:Wrong Password"], Severity.Error);
            return false;
        }

        if (result.Success)
        {
            string callbackUrl = string.Empty;
            string page = string.Empty;
            var currentUri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            if (QueryHelpers.ParseQuery(currentUri.Query).TryGetValue("callback", out var param))
                callbackUrl = param;
            if (QueryHelpers.ParseQuery(currentUri.Query).TryGetValue("page", out param))
                page = param;

            await cookieUtil.SetValueAsync("authToken", result.Result, secure: true);


            if (callbackUrl.IsNullOrEmpty())
                navigationManager.NavigateTo("/");
            else
                navigationManager.NavigateTo(
                    $"{callbackUrl}?atoken={result.Result.AccessToken}&rtoken={result.Result.RefreshToken}&vdate={result.Result.AccessTokenValidateTo}&page={page}");
            return true;
        }

        return false;
    }


    public async Task<bool> RegisterUserAsync(RegisterUserCommand command)
    {
        command.PhoneNumber = command.PhoneNumber.Trim();
        if (command.PhoneNumber.Length == 9) command.PhoneNumber = "+992" + command.PhoneNumber.Trim();
        else if (command.PhoneNumber.Length == 12 && command.PhoneNumber[0] != '+')
            command.PhoneNumber = "+" + command.PhoneNumber;

        var result = await httpClient.PostAsJsonAsync(configuration.GetIdentityUrl("Auth/register"), command);

        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"]);

        if (result.HttpStatusCode == HttpStatusCode.OK)
        {
            await LoginUserAsync(new LoginUserCommand
            {
                Password = command.Password,
                Username = command.Username
            });
            await userProfileService.Get();
            navigationManager.NavigateTo("/");
            return true;
        }

        return false;
    }

    public async Task<bool> ResetPassword(ResetPasswordCommand command)
    {
        var result = await httpClient.PostAsJsonAsync(configuration.GetIdentityUrl("Auth/ResetPassword"), command);
        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"],
            contentService["Profile:Resetpasswordsuccessfully"]);
        return result.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<UserDetailsResponse> CheckUserDetails(CheckUserDetailsQuery query)
    {
        var result =
            await httpClient.GetFromJsonAsync<UserDetailsResponse>(
                configuration.GetIdentityUrl(
                    $"User/CheckUserDetails/{query.UserName}/{query.PhoneNumber}/{query.Email}"));

        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"]);
        return result.Result;
    }

    public async Task ResendVerificationEmail()
    {
        var result = await httpClient.PostAsJsonAsync(configuration.GetIdentityUrl("Auth/VerifyEmail"), null!);
        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"]);
        if (result.HttpStatusCode == HttpStatusCode.OK)
        {
            snackbar.Add(contentService["Profile:Pleasecheckyouremail"], Severity.Info);
        }
    }

    public async Task<ApiResponse> SendVerificationEmailToken(string token, string userId)
    {
        var response = await httpClient.GetAsync(
                 configuration.GetIdentityUrl($"Auth/verify?token={WebUtility.UrlEncode(token)}&userid={userId}"));
        if (response.HttpStatusCode == HttpStatusCode.OK)
            return response;
        snackbar.ShowIfError(response,contentService["Profile:Servernotrespondingtry"]);
        return null;

    }
}