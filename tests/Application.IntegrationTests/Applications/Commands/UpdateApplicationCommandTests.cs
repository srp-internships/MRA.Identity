using MRA.Identity.Application.Contract.Applications.Commands;

namespace MRA.Jobs.Application.IntegrationTests.Applications.Commands;

public class UpdateApplicationCommandTests : BaseTest
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

    private readonly Identity.Domain.Entities.Application _app2 = new()
    {
        Slug = "slugForUpdate2",
        Name = "nameBase2",
        Description = "descriptionBase",
        ClientSecret = "there is secreeeeeeeeeeeeeeeeeeeeeeeeeeeeeet",
        CallbackUrls = ["http://localhost:2013"],
        IsProtected = false
    };

    private readonly Identity.Domain.Entities.Application _app3 = new()
    {
        Slug = "slugForUpdate3",
        Name = "nameBase3",
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
        if (_app2.Id == Guid.Empty)
            await AddEntity(_app2);
        if (_app3.Id == Guid.Empty)
            await AddEntity(_app3);
    }

    [Test]
    public async Task _ValidRequest__ReturnsOk_SaveInDb()
    {
        await _client.PutAsJsonAsync("/api/applications", new UpdateApplicationCommand
        {
            Name = "name2",
            Description = "",
            CallbackUrls =
                ["https://localhost"],
            Slug = _app1.Slug
        });
        var application = await GetEntity<Identity.Domain.Entities.Application>(s => s.Slug == _app1.Slug);
        application.Should().NotBeNull();
        application.ClientSecret.Should().NotBeNull();
    }

    [Test]
    public async Task _DuplicateName_ReturnsBadRequest()
    {
        // Arrange
        var command = new UpdateApplicationCommand
        {
            Name = _app3.Name,
            Description = "",
            CallbackUrls =
                ["https://localhost"],
            Slug = _app2.Slug
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/applications", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task _InvalidName_ReturnsBadRequest()
    {
        // Arrange
        var command = new UpdateApplicationCommand
        {
            Name = "",
            Description = "",
            CallbackUrls =
                ["https://localhost"],
            Slug = _app1.Slug
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/applications", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task _InvalidCallbackUrls_ReturnsBadRequest()
    {
        // Arrange
        var command = new UpdateApplicationCommand
        {
            Name = "name2",
            Description = "",
            CallbackUrls = ["asdfasdfasfd"],
            Slug = _app1.Slug
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/applications", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}