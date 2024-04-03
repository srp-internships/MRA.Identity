using System.Net;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.BlazorComponents.Snackbar.Extensions;
using MRA.Identity.Application.Contract.Applications.Commands;
using MRA.Identity.Application.Contract.Applications.Responses;
using MRA.Identity.Client.Services.ContentService;
using MudBlazor;

namespace MRA.Identity.Client.Services.Applications;

public class ApplicationsService(
    IHttpClientService httpClient,
    IConfiguration configuration,
    ISnackbar snackbar,
    IContentService contentService)
    : IApplicationsService
{
    private readonly string _serverNotRespMsg = contentService["Profile:Servernotrespondingtry"];
    private readonly string _successMsg = "Success";

    private const string ApplicationsEndpoint = "Applications";

    public async Task<bool> PostAsync(CreateApplicationCommand command)
    {
        var postResult = await httpClient.PostAsJsonAsync(configuration.GetIdentityUrl(ApplicationsEndpoint), command);
        snackbar.ShowIfError(postResult, _serverNotRespMsg, _successMsg);
        return postResult.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> PutAsync(UpdateApplicationCommand command)
    {
        var putResult = await httpClient.PutAsJsonAsync(configuration.GetIdentityUrl(ApplicationsEndpoint), command);
        snackbar.ShowIfError(putResult, _serverNotRespMsg, _successMsg);
        return putResult.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteAsync(string slug)
    {
        var deleteResult = await httpClient.DeleteAsync($"{configuration.GetIdentityUrl(ApplicationsEndpoint)}/{slug}");
        snackbar.ShowIfError(deleteResult, _serverNotRespMsg, _successMsg);
        return deleteResult.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<List<ApplicationResponse>> GetAllAsync()
    {
        var getResult =
            await httpClient.GetFromJsonAsync<List<ApplicationResponse>>(
                configuration.GetIdentityUrl(ApplicationsEndpoint));
        if (getResult.HttpStatusCode == HttpStatusCode.OK)
        {
            return getResult.Result;
        }

        snackbar.ShowIfError(getResult, _serverNotRespMsg);
        return [];
    }

    public async Task<ApplicationResponse> GetAsync(string slug)
    {
        var getResult =
            await httpClient.GetFromJsonAsync<ApplicationResponse>(
                $"{configuration.GetIdentityUrl(ApplicationsEndpoint)}/{slug}");
        if (getResult.HttpStatusCode == HttpStatusCode.OK)
        {
            return getResult.Result;
        }

        snackbar.ShowIfError(getResult, _serverNotRespMsg);
        return null!;
    }
}