namespace MRA.Identity.Application.Contract.EmailTemplates.Responses;

public class EmailTemplateResponse
{
    public required string Name { get; set; }
    public required string Subject { get; set; }
    public required string Slug { get; set; }
    public required string Text { get; set; }
}