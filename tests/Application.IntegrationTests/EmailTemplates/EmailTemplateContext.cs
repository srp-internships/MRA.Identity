namespace MRA.Jobs.Application.IntegrationTests.EmailTemplates;

public class EmailTemplateContext : BaseTest
{
    protected async Task AddTemplateAsync(string subject, string slug, string text)
    {
        await AddEntity(new EmailTemplate
        {
            Subject = subject,
            Text = text,
            Slug = slug
        });
    }
}