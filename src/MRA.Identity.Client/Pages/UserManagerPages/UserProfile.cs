using System.Net;
using Microsoft.AspNetCore.Components;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.BlazorComponents.Snackbar.Extensions;
using MRA.Identity.Application.Contract.Educations.Responses;
using MRA.Identity.Application.Contract.Experiences.Responses;
using MRA.Identity.Application.Contract.Profile.Responses;
using MRA.Identity.Application.Contract.Skills.Responses;
using MRA.Identity.Application.Contract.UserRoles.Commands;
using MRA.Identity.Application.Contract.UserRoles.Response;
using MRA.Identity.Client.Services.Profile;
using MRA.Identity.Client.Services.Roles;
using MudBlazor;

namespace MRA.Identity.Client.Pages.UserManagerPages;

public partial class UserProfile
{
    [Parameter] public string Username { get; set; }
    private List<UserRolesResponse> Roles { get; set; }
    [Inject] private IHttpClientService HttpClient { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private IConfiguration Configuration { get; set; }
    [Inject] private IUserProfileService UserProfileService { get; set; }
    [Inject] private IRoleService RoleService { get; set; }
    private string NewRoleName { get; set; }

    private UserProfileResponse _personalData = new();
    private UserSkillsResponse _userSkills = new();

    private List<UserExperienceResponse> _experiences = [];
    private List<UserEducationResponse> _educations = [];

    protected override async Task OnInitializedAsync()
    {
        await ReloadDataAsync();
        _personalData = await UserProfileService.Get(Username);
        _userSkills = await UserProfileService.GetUserSkills(Username);
        _experiences = await UserProfileService.GetExperiencesByUser(Username);
        _educations = await UserProfileService.GetEducationsByUser(Username);
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
}