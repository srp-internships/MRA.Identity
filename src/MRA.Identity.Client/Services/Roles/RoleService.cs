using System.Net;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.BlazorComponents.Snackbar.Extensions;
using MRA.Identity.Application.Contract.ApplicationRoles.Commands;
using MRA.Identity.Application.Contract.ApplicationRoles.Responses;
using MRA.Identity.Application.Contract.UserRoles.Commands;
using MRA.Identity.Application.Contract.UserRoles.Queries;
using MRA.Identity.Application.Contract.UserRoles.Response;
using MRA.Identity.Client.Services.ContentService;
using MudBlazor;

namespace MRA.Identity.Client.Services.Roles;

public class RoleService(
    IHttpClientService httpClient,
    IConfiguration configuration,
    ISnackbar snackbar,
    IContentService contentService) : IRoleService
{
    private const string RoleEndpoint = "Roles";
    private const string UserRolesEndpoint = "UserRoles";
    private readonly string _notRespMsg = contentService["Profile:Serverisnotresponding"];
    private readonly string _successMsg = contentService["Roles:Success"];


    public async Task<bool> Post(CreateRoleCommand command)
    {
        var response = await httpClient.PostAsJsonAsync(configuration.GetIdentityUrl(RoleEndpoint), command);
        snackbar.ShowIfError(response, _notRespMsg, _successMsg);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> Put(UpdateRoleCommand command)
    {
        var response =
            await httpClient.PutAsJsonAsync(configuration.GetIdentityUrl(RoleEndpoint) + "/" + command.Slug, command);
        snackbar.ShowIfError(response, _notRespMsg, _successMsg);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> Delete(string roleName)
    {
        var response = await httpClient.DeleteAsync(configuration.GetIdentityUrl(RoleEndpoint) + "/" + roleName);
        snackbar.ShowIfError(response, _notRespMsg, _successMsg);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<List<RoleNameResponse>> GetRoles()
    {
        var response =
            await httpClient.GetFromJsonAsync<List<RoleNameResponse>>(configuration.GetIdentityUrl(RoleEndpoint));
        snackbar.ShowIfError(response, _notRespMsg);
        return response.HttpStatusCode == HttpStatusCode.OK ? response.Result : [];
    }

    public async Task<List<UserRolesResponse>> GetUserRoles(GetUserRolesQuery query)
    {
        var response = await httpClient.GetFromJsonAsync<List<UserRolesResponse>>(
            configuration.GetIdentityUrl(UserRolesEndpoint) +
            $"?{nameof(query.UserName)}={query.UserName}&{nameof(query.Role)}={query.Role}");
        snackbar.ShowIfError(response, _notRespMsg);
        return response.HttpStatusCode == HttpStatusCode.OK ? response.Result : [];
    }

    public async Task<bool> DeleteUserRole(string userRoleSlug)
    {
        var response =
            await httpClient.DeleteAsync(configuration.GetIdentityUrl(UserRolesEndpoint) + "/" + userRoleSlug);
        snackbar.ShowIfError(response, _notRespMsg, _successMsg);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> PostUserRole(CreateUserRolesCommand command)
    {
        var response = await httpClient.PostAsJsonAsync(configuration.GetIdentityUrl(UserRolesEndpoint), command);
        snackbar.ShowIfError(response, _notRespMsg, _successMsg);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<List<UserRolesResponse>> GetAssignments(GetUserRolesQuery query)
    {
        var response =
            await httpClient.GetFromJsonAsync<List<UserRolesResponse>>(configuration.GetIdentityUrl(RoleEndpoint),
                query);
        snackbar.ShowIfError(response, _notRespMsg);
        return response.HttpStatusCode == HttpStatusCode.OK ? response.Result : [];
    }
}