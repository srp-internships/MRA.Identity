#nullable enable
namespace MRA.Identity.Domain.Entities;

public class Application
{
    public Guid Id { get; set; }
    public required string Slug { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = "";
    public required string ClientSecret { get; set; }
    public string[] CallbackUrls { get; set; }
    public bool IsProtected { get; set; }
    public ICollection<ApplicationUserLink> ApplicationUserLinks { get; set; }
    public Guid DefaultRoleId { get; set; }
    public ApplicationRole? DefaultRole { get; set; }
}