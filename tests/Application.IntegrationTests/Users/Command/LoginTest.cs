﻿using MRA.Identity.Application.Contract.User.Commands.LoginUser;
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
            Username = NewUser.UserName,
            Password = NewUserPassword,
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
    public async Task UserIsNotInApplication_ShouldCreateApplicationUserLink()
    {
        var user = new ApplicationUser()
            { UserName = @"John", Email = "John@example.com", FirstName = "John", LastName = "John" };
        await AddUser(user, "Password12#1");

        var request = new LoginUserCommand
        {
            Username = user.UserName,
            Password = "Password12#1",
            ApplicationId = _application.Id,
            CallBackUrl = _application.CallbackUrls.First()
        };
        (await _client.PostAsJsonAsync("api/auth/login", request)).EnsureSuccessStatusCode();
        var applicationUserLink =
            await GetEntity<ApplicationUserLink>(s => s.UserId == user.Id && s.ApplicationId == _application.Id);
        applicationUserLink.Should().NotBeNull();
    }

    [Test]
    public async Task UserIsNotInApplicationAndAppIsProtected_ShouldNotLogin()
    {
        var application = new MRA.Identity.Domain.Entities.Application
        {
            Slug = "test-app-sluuuuuug",
            Name = "name of protected application for " +
                   nameof(UserIsNotInApplicationAndAppIsProtected_ShouldNotLogin),
            Description = "this is a desc",
            ClientSecret = "this is a secret",
            CallbackUrls = ["https://localhost"],
            IsProtected = true,
            DefaultRoleId = _application.DefaultRoleId
        };
        await AddEntity(application);
        var user = new ApplicationUser
            { UserName = @"John1", Email = "Jo1hn@example.com", FirstName = "Jo1hn", LastName = "Joh1n" };
        await AddUser(user, "Password12#1");

        var request = new LoginUserCommand
        {
            Username = user.UserName,
            Password = "Password12#1",
            ApplicationId = application.Id,
            CallBackUrl = application.CallbackUrls.First()
        };
        (await _client.PostAsJsonAsync("api/auth/login", request)).StatusCode.Should().NotBe(HttpStatusCode.OK);
        var applicationUserLink =
            await GetEntity<ApplicationUserLink>(s => s.UserId == user.Id && s.ApplicationId == application.Id);
        applicationUserLink.Should().BeNull();
    }

    [Test]
    public async Task UserIsInApplication_ShouldNotCreateApplicationUserLink()
    {
        var user = new ApplicationUser()
            { UserName = @"John1", Email = "John1@example.com", FirstName = "Joh1n", LastName = "Jo1hn" };
        await AddUser(user, "Password12#1");

        var applicationUserLink = new ApplicationUserLink
        {
            UserId = user.Id,
            ApplicationId = _application.Id
        };
        await AddEntity(applicationUserLink);

        var request = new LoginUserCommand
        {
            Username = user.UserName,
            Password = "Password12#1",
            ApplicationId = _application.Id,
            CallBackUrl = _application.CallbackUrls.First()
        };

        (await _client.PostAsJsonAsync("api/auth/login", request)).EnsureSuccessStatusCode();
        var userApplicationUserLinks =
            await GetWhere<ApplicationUserLink>(s => s.ApplicationId == _application.Id && s.UserId == user.Id);
        userApplicationUserLinks.Count.Should().Be(1);
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
    public async Task Login_SuperAdmin_ReturnsReturnsOk()
    {
        // Arrange
        var request = new LoginUserCommand
        {
            Username = "SuperAdmin", Password = "Mra123!!@#$AGfer4",
            ApplicationId = Guid.Empty,
            CallBackUrl = "httpasdfaaas;l;;"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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