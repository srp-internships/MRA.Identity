using MRA.Identity.Application.Contract.EmailTemplates.Responses;

namespace MRA.Jobs.Application.IntegrationTests.EmailTemplates.Queries;

public class GetSubjectsQueryTests : EmailTemplateContext
{
    private async Task AddTemplateAsync(string subject, string slug, string text)
    {
        await AddEntity(new EmailTemplate
        {
            Subject = subject,
            Text = text,
            Slug = slug
        });
    }

    [Test]
    public async Task _Request_ReturnsOk()
    {
        await AddTemplateAsync("1", "1", "1");
        await AddTemplateAsync("2", "2", "2");
        await AddTemplateAsync("3", "3", "3");
        await AddTemplateAsync("4", "4", "4");
        await AddTemplateAsync("5", "5", "5");
        var subjectsResponse =
            await _client.GetFromJsonAsync<List<EmailTemplateSubjectResponse>>("api/emailTemplates/getSubjects");
        Assert.That(subjectsResponse.FirstOrDefault(s => s.Slug == "1" && s.Subject == "1"), Is.Not.Null);
        Assert.That(subjectsResponse.FirstOrDefault(s => s.Slug == "2" && s.Subject == "2"), Is.Not.Null);
        Assert.That(subjectsResponse.FirstOrDefault(s => s.Slug == "3" && s.Subject == "3"), Is.Not.Null);
        Assert.That(subjectsResponse.FirstOrDefault(s => s.Slug == "4" && s.Subject == "4"), Is.Not.Null);
        Assert.That(subjectsResponse.FirstOrDefault(s => s.Slug == "5" && s.Subject == "5"), Is.Not.Null);
    }
}