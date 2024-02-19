using MRA.Identity.Application.Contract.EmailTemplates.Responses;

namespace MRA.Jobs.Application.IntegrationTests.EmailTemplates.Queries;

public class GetTemplateQueryTests : EmailTemplateContext
{
    [Test]
    public async Task _Request_ReturnsOk()
    {
        await AddTemplateAsync("name9", "10", "10", "10");
        var templateResponse =
            await _client.GetFromJsonAsync<EmailTemplateResponse>("api/emailTemplates/10");
        Assert.That(templateResponse != null);
        Assert.That(templateResponse?.Subject == "10");
        Assert.That(templateResponse?.Slug == "10");
        Assert.That(templateResponse?.Name == "name9");
        Assert.That(templateResponse?.Text == "10");
    }


    [Test]
    public async Task _NotExistSlug_ReturnsNotFound()
    {
        var templateResponse =
            await _client.GetAsync(
                "api/emailTemplates/getTemplate/fhaiduhuiakdjfahkeuiakjsfhlwe");
        Assert.That(templateResponse.StatusCode == HttpStatusCode.NotFound);
    }
}