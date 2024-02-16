using MRA.Identity.Application.Contract.EmailTemplates.Commands;

namespace MRA.Jobs.Application.IntegrationTests.EmailTemplates.Commands;

public class CreateEmailTemplateCommandTests : EmailTemplateContext
{
    [Test]
    public async Task _ValidRequest_ReturnsOkSaveIntoDb()
    {
        var request = new CreateEmailTemplateCommand
        {
            Subject = "this is not a random subject",
            Text = "this is a text of this email"
        };
        var response = await _client.PostAsJsonAsync("api/emailTemplates", request);
        response.EnsureSuccessStatusCode();

        var templateSlug = await response.Content.ReadAsStringAsync();
        var dbValue = await GetEntity<EmailTemplate>(s => s.Slug == templateSlug);

        Assert.That(dbValue, Is.Not.Null);
        Assert.That(dbValue.Text == request.Text);
        Assert.That(dbValue.Subject == request.Subject);
    }

    [Test]
    public async Task _ValidDuplicateSubject_ReturnsBadRequestDontSaveIntoDb()
    {
        var templateEntity = new EmailTemplate
        {
            Subject = "it will be duplicate",
            Text = "does not matter",
            Slug = "slug-really"
        };
        await AddEntity(templateEntity);

        var request = new CreateEmailTemplateCommand
        {
            Subject = templateEntity.Slug,
            Text = templateEntity.Text
        };

        var response = await _client.PostAsJsonAsync("api/emailTemplates", request);
        Assert.That(response.StatusCode == HttpStatusCode.BadRequest);

        var emailTemplates = await GetWhere<EmailTemplate>(s => s.Subject == templateEntity.Subject);
        Assert.That(emailTemplates.Count == 1); //not added if duplicate subject
    }
}