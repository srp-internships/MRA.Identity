namespace MRA.Identity.Application.Common.Interfaces.Services;

public interface ISlugService
{
    string GenerateSlug(string raw);
}