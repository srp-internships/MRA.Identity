using FluentValidation;
using Microsoft.Extensions.Localization;
using MRA.Identity.Application.Contract.Resources;

namespace MRA.Identity.Application.Contract.ContentService;

public class ContentService(
    IStringLocalizer<RussianValidatorMessagase> russian,
    IStringLocalizer<EnglishValidatorMessagase> english) : IContentService
{
    public string this[string name]
    {
        get
        {
            if (ValidatorOptions.Global.LanguageManager.Culture?.Name.Contains("ru",
                    StringComparison.OrdinalIgnoreCase) ?? false)
                return russian[name].Value;

            return english[name].Value;
        }
    }
}