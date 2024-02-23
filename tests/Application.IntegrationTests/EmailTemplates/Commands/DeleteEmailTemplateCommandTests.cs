namespace MRA.Jobs.Application.IntegrationTests.EmailTemplates.Commands;

public class DeleteEmailTemplateCommandTests : EmailTemplateContext
{
    [Test]
    public async Task _ValidRequest_ReturnsOkSaveIntoDb()
    {
        var slug = "this is a slug number 10000009";
        await AddTemplateAsync("name1", "this is not a random sasdfubject ten", slug,
            "this is a text of this emaisfafdsfasdfasdfdfaal");

        var response = await _client.DeleteAsync("api/emailTemplates/" + slug);
        response.EnsureSuccessStatusCode();

        var dbValue = await GetEntity<EmailTemplate>(s => s.Slug == slug);

        Assert.That(dbValue, Is.Null);
    }

    [Test]
    public async Task _NotExistRequest_ReturnsNotFoundSaveIntoDb()
    {
        var slug = "this is a slug number 10000078";
        await AddTemplateAsync("name2", "request.Subject", slug + "act", "request.Text");

        var response = await _client.DeleteAsync("api/emailTemplates/" + slug);
        Assert.That(response.StatusCode == HttpStatusCode.NotFound);
    }
}