using MRA.Identity.Application.Contract.Applications.Commands;

namespace MRA.Jobs.Application.IntegrationTests.Applications.Commands;

public class CreateApplicationCommandTests : BaseTest
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
    public async Task _ValidRequest__ReturnsOk_GenerateSecret_SaveInDb()
    {
        await _client.PostAsJsonAsync("/api/applications", new CreateApplicationCommand
        {
            Name = "name1",
            Description = "",
            CallbackUrls =
                ["https://localhost"]
        });
        var application = await GetEntity<Identity.Domain.Entities.Application>(s => s.Name == "name1");
        application.Should().NotBeNull();
        application.ClientSecret.Should().NotBeNull();
    }

    [Test]
    public async Task _InvalidName_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateApplicationCommand
        {
            Name = "",
            Description = "",
            CallbackUrls =
                ["https://localhost"]
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/applications", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task _InvalidCallbackUrls_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateApplicationCommand
        {
            Name = "name1",
            Description = "",
            CallbackUrls =
                ["", "12", "13", "14", "15", "16", "17"]
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/applications", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task _DuplicateName_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateApplicationCommand
        {
            Name = _app1.Name,
            Description = "",
            CallbackUrls =
                ["https://localhost"],
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/applications", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}