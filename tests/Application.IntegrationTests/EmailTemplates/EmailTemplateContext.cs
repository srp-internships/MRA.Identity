namespace MRA.Jobs.Application.IntegrationTests.EmailTemplates;

public abstract class EmailTemplateContext : BaseTest
{
    [SetUp]
    public async Task SetUp()
    {
        await AddAdminAuthorizationAsync();
    }

    protected async Task AddTemplateAsync(string name, string subject, string slug, string text)
    {
        await AddEntity(new EmailTemplate
        {
            Subject = subject,
            Text = text,
            Slug = slug,
            Name = name
        });
    }
}