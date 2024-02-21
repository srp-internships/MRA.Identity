using System.Net;
using Microsoft.AspNetCore.Components;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.BlazorComponents.Snackbar.Extensions;
using MRA.Identity.Application.Contract.EmailTemplates.Responses;
using MudBlazor;

namespace MRA.Identity.Client.Pages.EmailTemplates;

public partial class EmailTemplatesPage
{
    [Inject] private IHttpClientService HttpClient { get; set; }
    [Inject] private IConfiguration Configuration { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    public List<EmailTemplateNamesResponse> NamesResponses { get; set; }

    [Inject] private IDialogService DialogService { get; set; }


    private async void DeleteConfirm(string slug)
    {
        var result = await DialogService.ShowMessageBox(
            "Warning",
            "Confirm deletion",
            yesText: "Delete", cancelText: "Cancel");
        if (result == true)
        {
            var deleteAsync = await HttpClient.DeleteAsync(Configuration.GetIdentityUrl($"emailTemplates/{slug}"));
            Snackbar.ShowIfError(deleteAsync, ContentService["Profile:Servernotrespondingtry"],
                "Template was successfully removed");
            await OnInitializedAsync();
        }

        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        var getResult =
            await HttpClient.GetFromJsonAsync<List<EmailTemplateNamesResponse>>(
                Configuration.GetIdentityUrl("emailTemplates"));
        Snackbar.ShowIfError(getResult, ContentService["Profile:Servernotrespondingtry"]);
        if (getResult.HttpStatusCode == HttpStatusCode.OK)
        {
            NamesResponses = getResult.Result;
        }
    }
}