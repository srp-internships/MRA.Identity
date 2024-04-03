namespace MRA.Jobs.Application.IntegrationTests.Applications.Commands;

public class DeleteApplicationCommandTests : BaseTest
{
    private readonly Identity.Domain.Entities.Application _app1 = new()
    {
        Slug = "slugForUpdate",
        Name = "nameBase",
        Description = "descriptionBase",
        ClientSecret = "there is secreeeeeeeeeeeeeeeeeeeeeeeeeeeeeet",
        CallbackUrls = ["http://localhost:2013"],
        IsProtected = false
    };

    [SetUp]
    public async Task _SetUp()
    {
        await AddAuthorizationAsync();
        if (_app1.Id == Guid.Empty)
            await AddEntity(_app1);
    }

    [Test]
    public async Task _ValidRequest_ReturnsOk()
    {
        var response = await _client.DeleteAsync($"api/applications/{_app1.Slug}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task _NotExistSlug_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"api/applications/unexistSlug");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}