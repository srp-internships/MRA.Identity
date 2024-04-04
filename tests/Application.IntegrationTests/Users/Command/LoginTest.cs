using MRA.Identity.Application.Contract.User.Commands.LoginUser;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Jobs.Application.IntegrationTests.Users.Command;

[TestFixture]
public class LoginTest : BaseTest
{
    private readonly ApplicationRole _role = new() { Name = "Reviewer" + nameof(LoginTest) };

    private readonly MRA.Identity.Domain.Entities.Application _application = new()
    {
        Slug = "Application" + nameof(LoginTest),
        Name = "Application" + nameof(LoginTest),
        ClientSecret = "fa;sldkjfalskjdfoqwijf;odsnfoweifneronflkfn;doifneoio",
        CallbackUrls = ["https://localhost"]
    };

    public override async Task OneTimeSetup()
    {
        await base.OneTimeSetup();
        _role.NormalizedName = _role.Name?.ToUpper();
        _role.Slug = _role.NormalizedName;
        await AddEntity(_role);
        _application.DefaultRoleId = _role.Id;
        await AddEntity(_application);
    }

    [Test]
    public async Task Login_RequestWithCorrectLoginData_ReturnsOk()
    {
        // Arrange
        var request = new LoginUserCommand
        {
            Username = "@Alex33",
            Password = "password@#12P",
            ApplicationId = _application.Id,
            CallBackUrl = _application.CallbackUrls.First()
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/Auth/login", request);

        // Assert
        var jwt = await response.Content.ReadFromJsonAsync<JwtTokenResponse>();
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(jwt?.AccessToken, Is.Not.Null.Or.Empty);
        });
    }

    [Test]
    public async Task Login_RequestWithEmptyLoginData_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginUserCommand
        {
            Username = "null", Password = "12345",
            ApplicationId = _application.Id,
            CallBackUrl = _application.CallbackUrls.First()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [TestCase("@Alex22", "password")]
    [TestCase("@Alex22", "ejehfefhuehf")]
    [TestCase("@Alex", "fesijfwer11")]
    [TestCase("@Alex", "password@#12P")]
    public async Task Login_RequestWithIncorrectLoginData_ReturnsUnauthorized(string username, string password)
    {
        // Arrange
        var request = new LoginUserCommand
        {
            Username = username, Password = password,
            ApplicationId = _application.Id,
            CallBackUrl = _application.CallbackUrls.First()
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/Auth/login", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}