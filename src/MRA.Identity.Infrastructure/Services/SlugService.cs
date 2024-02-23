using MRA.Identity.Application.Common.Interfaces.Services;
using Slugify;

namespace MRA.Identity.Infrastructure.Services;

public class SlugService : ISlugService
{
    private static readonly SlugHelper SlugHelper = new();

    public string GenerateSlug(string raw)
    {
        var translatedText = NickBuhro.Translit.Transliteration.CyrillicToLatin(raw);

        return SlugHelper.GenerateSlug(translatedText);
    }
}