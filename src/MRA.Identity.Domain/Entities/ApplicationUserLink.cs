namespace MRA.Identity.Domain.Entities;
#nullable enable
public class ApplicationUserLink
{
    public Guid Id { get; set; }

    public required Guid ApplicationId { get; set; }
    public Application? Application { get; set; }

    public required Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
}