namespace MRA.Identity.Domain.Entities;

public class EmailTemplate
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Subject { get; set; }
    public required string Text { get; set; }
    public required string Slug { get; set; }
}