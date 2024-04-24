using Microsoft.AspNetCore.Components;
using MRA.Identity.Application.Contract.Educations.Responses;
using MRA.Identity.Application.Contract.Experiences.Responses;
using MRA.Identity.Application.Contract.Profile.Responses;
using MRA.Identity.Application.Contract.Skills.Responses;
using MRA.Identity.Application.Contract.UserRoles.Commands;
using MRA.Identity.Application.Contract.UserRoles.Queries;
using MRA.Identity.Application.Contract.UserRoles.Response;
using MRA.Identity.Client.Services.Profile;
using MRA.Identity.Client.Services.Roles;

namespace MRA.Identity.Client.Pages.UserManagerPages;

public partial class UserProfile
{
    [Parameter] public string Username { get; set; }
    private List<UserRolesResponse> UserRoles { get; set; }
    [Inject] private IUserProfileService UserProfileService { get; set; }
    [Inject] private IRoleService RoleService { get; set; }
    private string NewRoleName { get; set; }

    private UserProfileResponse _personalData = new();
    private UserSkillsResponse _userSkills = new();

    private List<UserExperienceResponse> _experiences = [];
    private List<UserEducationResponse> _educations = [];
    private List<Application.Contract.ApplicationRoles.Responses.RoleNameResponse> _roleNames = [];

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
        UserRoles = await RoleService.GetUserRoles(new GetUserRolesQuery { UserName = Username });
        _roleNames = await RoleService.GetRoles();
    }

    private async Task OnDeleteClick(string contextSlug)
    {
        if (!string.IsNullOrWhiteSpace(contextSlug))
        {
            var deleteSuccess = await RoleService.DeleteUserRole(contextSlug);
            if (deleteSuccess)
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

            await RoleService.PostUserRole(userRoleCommand);

            await ReloadDataAsync();
            StateHasChanged();
        }
    }
}