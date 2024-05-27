namespace MRA.Identity.Application.Contract.ContentService;

public interface IContentService
{
    string this[string name] { get; }
}