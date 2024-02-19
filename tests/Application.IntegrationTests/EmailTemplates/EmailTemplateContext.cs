namespace MRA.Jobs.Application.IntegrationTests.EmailTemplates;

public abstract class EmailTemplateContext : BaseTest
{
    [SetUp]
    public async Task SetUp()
    {
        await AddReviewerAuthorizationAsync();
    }

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