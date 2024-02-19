using MRA.Identity.Application.Contract.EmailTemplates.Responses;

namespace MRA.Jobs.Application.IntegrationTests.EmailTemplates.Queries;

public class GetSubjectsQueryTests : EmailTemplateContext
{
    [Test]
    public async Task _Request_ReturnsOk()
    {
        await AddTemplateAsync("name3", "1", "1", "1");
        await AddTemplateAsync("name4", "2", "2", "2");
        await AddTemplateAsync("name5", "3", "3", "3");
        await AddTemplateAsync("name6", "4", "4", "4");
        await AddTemplateAsync("name7", "5", "5", "5");
        var subjectsResponse =
            await _client.GetFromJsonAsync<List<EmailTemplateNamesResponse>>("api/emailTemplates/getSubjects");
        Assert.That(subjectsResponse.FirstOrDefault(s => s.Slug == "1" && s.Name == "name3"), Is.Not.Null);
        Assert.That(subjectsResponse.FirstOrDefault(s => s.Slug == "2" && s.Name == "name4"), Is.Not.Null);
        Assert.That(subjectsResponse.FirstOrDefault(s => s.Slug == "3" && s.Name == "name5"), Is.Not.Null);
        Assert.That(subjectsResponse.FirstOrDefault(s => s.Slug == "4" && s.Name == "name6"), Is.Not.Null);
        Assert.That(subjectsResponse.FirstOrDefault(s => s.Slug == "5" && s.Name == "name7"), Is.Not.Null);
    }
}