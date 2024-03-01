using Blazored.TextEditor;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.BlazorComponents.Snackbar.Extensions;
using MRA.Identity.Application.Contract.EmailTemplates.Commands;
using MudBlazor;

namespace MRA.Identity.Client.Pages.EmailTemplates;

public partial class CreateEmailTemplate
{
    [Inject] private IHttpClientService HttpClientService { get; set; }
    [Inject] private IConfiguration Configuration { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }


    private readonly CreateEmailTemplateCommand _model = new()
    {
        Name = "",
        Subject = "",
        Text = ""
    };

    private MudForm _form;
    private readonly CreateEmailTemplateCommandValidator _validator = new();

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
            await HttpClientService.PostAsJsonAsync(Configuration.GetIdentityUrl("emailTemplates"), _model);
        Snackbar.ShowIfError(postResult, ContentService["Profile:Servernotrespondingtry"],
            "Template was successfully created");
    }
}

public class CreateEmailTemplateCommandValidator : AbstractValidator<CreateEmailTemplateCommand>
{
    public CreateEmailTemplateCommandValidator()
    {
        RuleFor(s => s.Name).NotEmpty().NotNull();
        RuleFor(s => s.Subject).NotEmpty().NotNull();
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result =
            await ValidateAsync(
                ValidationContext<CreateEmailTemplateCommand>.CreateWithOptions((CreateEmailTemplateCommand)model,
                    x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}