using System.Net;
using Blazored.TextEditor;
using Microsoft.AspNetCore.Components;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.BlazorComponents.Snackbar.Extensions;
using MRA.Identity.Application.Contract.EmailTemplates.Commands;
using MudBlazor;

namespace MRA.Identity.Client.Pages.EmailTemplates;

public partial class EditEmailTemplate
{
    [Parameter] public string Slug { get; set; }
    [Inject] private IHttpClientService HttpClientService { get; set; }
    [Inject] private IConfiguration Configuration { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }


    private UpdateEmailTemplateCommand _model;

    private MudForm _form;


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var response = await HttpClientService.GetFromJsonAsync<UpdateEmailTemplateCommand>(
                Configuration.GetIdentityUrl($"emailTemplates/{Slug}"));
            Snackbar.ShowIfError(response, ContentService["Profile:Servernotrespondingtry"]);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                _model = response.Result;
                Console.WriteLine(_model.Text);
                
                await _quillHtml.LoadHTMLContent(_model.Text);
            }
        }
    }

    private BlazoredTextEditor _quillHtml;
    private string _imageLinkToInsertToEditor;

    public async void InsertImage()
    {
        if (!string.IsNullOrEmpty(_imageLinkToInsertToEditor))
        {
            await _quillHtml.InsertImage(_imageLinkToInsertToEditor);
            StateHasChanged();
        }
    }

    private async Task Submit()
    {
        var html = await _quillHtml.GetHTML();
        _model.Text = html;
        var postResult =
            await HttpClientService.PutAsJsonAsync(Configuration.GetIdentityUrl("emailTemplates"), _model);
        Snackbar.ShowIfError(postResult, ContentService["Profile:Servernotrespondingtry"],
            "Template was successfully updated");
    }
}