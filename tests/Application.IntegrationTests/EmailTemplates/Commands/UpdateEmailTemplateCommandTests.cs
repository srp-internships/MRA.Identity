using MRA.Identity.Application.Contract.EmailTemplates.Commands;

namespace MRA.Jobs.Application.IntegrationTests.EmailTemplates.Commands;

public class UpdateEmailTemplateCommandTests : EmailTemplateContext
{
    [Test]
    public async Task _ValidRequest_ReturnsOkSaveIntoDb()
    {
        var request = new UpdateEmailTemplateCommand
        {
            Subject = "this is not a random subject ten",
            Text = "this is a text of this email",
            Slug = "this is a slug number 1000009"
        };
        await AddTemplateAsync(request.Subject, request.Slug, request.Text);
        request.Text += "fsdasdfsa";
        request.Subject += "fsdas subject dfsa";

        var response = await _client.PutAsJsonAsync("api/emailTemplates", request);
        response.EnsureSuccessStatusCode();

        var templateSlug = await response.Content.ReadAsStringAsync();
        var dbValue = await GetEntity<EmailTemplate>(s => s.Slug == templateSlug);

        Assert.That(dbValue, Is.Not.Null);
        Assert.That(dbValue.Text == request.Text);
        Assert.That(dbValue.Subject == request.Subject);
    }

    [Test]
    public async Task _NotExistRequest_ReturnsNotFoundSaveIntoDb()
    {
        var request = new UpdateEmailTemplateCommand
        {
            Subject = "this is not a random subject twenty",
            Text = "this is a text of this email2",
            Slug = "this is a slug number 1000001"
        };
        await AddTemplateAsync(request.Subject, request.Slug + "act", request.Text);
        request.Text += "fsdasdfsa";
        request.Subject += "fsdas subject dfsa";

        var response = await _client.PutAsJsonAsync("api/emailTemplates", request);
        Assert.That(response.StatusCode == HttpStatusCode.NotFound);
    }
}