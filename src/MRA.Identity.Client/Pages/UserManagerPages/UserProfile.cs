using System.Net;
using Microsoft.AspNetCore.Components;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.BlazorComponents.Snackbar.Extensions;
using MRA.Identity.Application.Contract.Claim.Commands;
using MRA.Identity.Application.Contract.Claim.Responses;
using MRA.Identity.Application.Contract.Educations.Responses;
using MRA.Identity.Application.Contract.Experiences.Responses;
using MRA.Identity.Application.Contract.Profile.Responses;
using MRA.Identity.Application.Contract.Skills.Responses;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Application.Contract.UserRoles.Commands;
using MRA.Identity.Application.Contract.UserRoles.Response;
using MRA.Identity.Client.Services.Profile;
using MudBlazor;

namespace MRA.Identity.Client.Pages.UserManagerPages;

public partial class UserProfile
{
    [Parameter] public string Username { get; set; }
    private List<UserRolesResponse> Roles { get; set; }
    private List<UserClaimsResponse> UserClaims { get; set; }

    private List<UserClaimsResponse> ReadOnlyClaims { get; set; } =
    [
        new UserClaimsResponse { ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/application" },
        new UserClaimsResponse { ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" },
        new UserClaimsResponse { ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/id" },
        new UserClaimsResponse { ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/username" },
        new UserClaimsResponse { ClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/email" }
    ];

    [Inject] private IHttpClientService HttpClient { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private IConfiguration Configuration { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] private IUserProfileService UserProfileService { get; set; }
    private string NewRoleName { get; set; }

    private UserProfileResponse _personalData = new();
    private UserSkillsResponse _userSkills = new();

    private List<UserExperienceResponse> _experiences = new();
    private List<UserEducationResponse> _educations = new();
    private string _claimType;
    private string _claimValue;
    private UserResponse _user;
    private bool _loader;
  
    protected override async Task OnInitializedAsync()
    {
        await ReloadDataAsync();
        await ReloadUserClaimsAsync();
        _personalData = await UserProfileService.Get(Username);
        _userSkills = await UserProfileService.GetUserSkills(Username);
        _experiences = await UserProfileService.GetExperiencesByUser(Username);
        _educations = await UserProfileService.GetEducationsByUser(Username);
        _user = (await HttpClient.GetFromJsonAsync<UserResponse>(Configuration.GetIdentityUrl($"User/{Username}")))
            .Result;
    }

    private async Task ReloadUserClaimsAsync()
    {
        _claimType = "";
        _claimValue = "";
        var userClaimsResponse =
            await HttpClient.GetFromJsonAsync<List<UserClaimsResponse>>(
                Configuration.GetIdentityUrl($"Claims?username={Username}"));
        if (userClaimsResponse.HttpStatusCode == HttpStatusCode.OK)
        {
            UserClaims = userClaimsResponse.Result?.Where(s => ReadOnlyClaims.All(f => f.ClaimType != s.ClaimType))
                .ToList();
            ReadOnlyClaims.ForEach(s =>
                s.ClaimValue = userClaimsResponse.Result?.FirstOrDefault(f => f.ClaimType == s.ClaimType)
                    ?.ClaimValue);
        }
    }

    private async Task ReloadDataAsync()
    {
        var userRolesResponse =
            await HttpClient.GetFromJsonAsync<List<UserRolesResponse>>(
                Configuration.GetIdentityUrl($"UserRoles?userName={Username}"));
        if (userRolesResponse.HttpStatusCode == HttpStatusCode.OK)
            Roles = userRolesResponse.Result;
    }

    private async Task OnDeleteClick(string contextSlug)
    {
        if (!string.IsNullOrWhiteSpace(contextSlug))
        {
            var deleteResult = await HttpClient.DeleteAsync(Configuration.GetIdentityUrl($"UserRoles/{contextSlug}"));
            Snackbar.ShowIfError(deleteResult, ContentService["Profile:ServerIsNotResponding"]);

            if (deleteResult.HttpStatusCode == HttpStatusCode.OK)
            {
                await ReloadDataAsync();
                StateHasChanged();
            }
        }
    }

    private async Task OnAddClick()
    {
        if (!string.IsNullOrWhiteSpace(NewRoleName))
        {
            var userRoleCommand = new CreateUserRolesCommand { RoleName = NewRoleName, UserName = Username };

            var userRoleResponse =
                await HttpClient.PostAsJsonAsync(Configuration.GetIdentityUrl("UserRoles"), userRoleCommand);
            Snackbar.ShowIfError(userRoleResponse, ContentService["Profile:ServerIsNotResponding"]);

            await ReloadDataAsync();
            StateHasChanged();
        }
    }

    private async Task OnAddClaimClick()
    {
        if (!string.IsNullOrWhiteSpace(_claimType) && !string.IsNullOrWhiteSpace(_claimValue))
        {
            var command =
                new CreateClaimCommand { ClaimType = _claimType, ClaimValue = _claimValue, UserId = _user.Id };

            _loader = true;
            Snackbar.ShowIfError(await HttpClient.PostAsJsonAsync(Configuration.GetIdentityUrl("Claims"), command),
                ContentService["Profile:ServerIsNotResponding"]);
            await ReloadUserClaimsAsync();
            _claimType = "";
            _claimValue = "";
            _loader = false;
            StateHasChanged();
        }
    }

    private async Task OnDeleteClaimClick(string slug)
    {
        var deleteResult = await HttpClient.DeleteAsync(Configuration.GetIdentityUrl($"Claims/{slug}"));
        Snackbar.ShowIfError(deleteResult, ContentService["Profile:ServerIsNotResponding"]);

        await ReloadUserClaimsAsync();
        StateHasChanged();
    }
}