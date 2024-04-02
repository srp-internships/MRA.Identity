namespace MRA.Identity.Application.Contract.Applications.Responses;

public class ApplicationResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; }
    public required string ClientSecret { get; set; }
    public string[] CallbackUrls { get; set; }
    public bool IsProtected { get; set; }
    public required string Slug { get; set; }
}