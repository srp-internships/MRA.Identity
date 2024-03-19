using System.Globalization;
using FluentValidation;
using MRA.Identity.Application.Contract.ContentService;

namespace MRA.Identity.Application.Contract;

public static class SetLocalization
{
    public static void SetCulture(string name)
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo(name);
    }
}