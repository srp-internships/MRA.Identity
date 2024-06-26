﻿using MRA.Identity.Application.Contract.User.Commands.LoginUser;
using MRA.Identity.Application.Contract.User.Commands.RegisterUser;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Jobs.Application.IntegrationTests.AccessToken;

[TestFixture]
public class GetAccessTokenIntegrationTests : BaseTest
{
    private JwtTokenResponse LoginResponse { get; set; }
    private readonly ApplicationRole _role = new() { Name = "Reviewer" + nameof(GetAccessTokenIntegrationTests) };

    private readonly MRA.Identity.Domain.Entities.Application _application =
        new MRA.Identity.Domain.Entities.Application
        {
            Slug = "Application" + nameof(GetAccessTokenIntegrationTests),
            Name = "Application" + nameof(GetAccessTokenIntegrationTests),
            ClientSecret = "fa;sldkjfalskjdfoqwijf;odsnfoweifneronflkfn;doifneoio",
            CallbackUrls = ["https://localhost"],
            Description = ""
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

    [SetUp]
    public async Task SetUp()
    {
        if (LoginResponse == null)
        {
            var registerCommand1 = new RegisterUserCommand
            {
                Email = "test123@example.com",
                Password = "password@#12P",
                FirstName = "Alex",
                Username = "@Alex111122",
                LastName = "Makedonsky",
                PhoneNumber = "+992123156789",
                ConfirmPassword = "password@#12P",
                ApplicationId = _application.Id,
                CallBackUrl = _application.CallbackUrls.First()
            };
            var res = await _client.GetAsync($"api/sms/send_code?PhoneNumber={registerCommand1.PhoneNumber}");
            res.EnsureSuccessStatusCode();

            registerCommand1.VerificationCode =
                (await GetEntity<ConfirmationCode>(x => x.PhoneNumber == registerCommand1.PhoneNumber)).Code;
            var loginCommand1 = new LoginUserCommand
            {
                Username = "@Alex111122", Password = "password@#12P", ApplicationId = _application.Id,
                CallBackUrl = _application.CallbackUrls.First()
            };

            await RegisterUser(registerCommand1);
            LoginResponse = await LoginUser(loginCommand1);
        }
    }

    [Test]
    public async Task Refresh_ValidRefreshAndAccessToken_OkResult()
    {
        //Arrange
        var request = new GetAccessTokenUsingRefreshTokenQuery()
        {
            AccessToken = LoginResponse.AccessToken, RefreshToken = LoginResponse.RefreshToken
        };
        await AddAuthorizationAsync();
        //Act
        var response = await _client.PostAsJsonAsync("api/Auth/refresh", request);

        //Assert
        response.EnsureSuccessStatusCode();
        Assert.That(response, Is.Not.Null);
    }

    [Test]
    public async Task Refresh_InvalidRefreshToken_BadRequestResult()
    {
        //Arrange
        var request = new GetAccessTokenUsingRefreshTokenQuery
        {
            AccessToken = LoginResponse.AccessToken, RefreshToken = "sdf" //invalid refreshToken
        };

        //Act
        var response = await _client.PostAsJsonAsync("api/Auth/refresh", request);

        //Assert
        response.IsSuccessStatusCode.Should().Be(false);
    }

    [Test]
    public async Task Refresh_InvalidAccessToken_BadRequestResult()
    {
        //Arrange
        var request = new GetAccessTokenUsingRefreshTokenQuery
        {
            AccessToken = "sdf", //invalid accessToken
            RefreshToken = LoginResponse.RefreshToken,
        };

        //Act
        var response = await _client.PostAsJsonAsync("api/Auth/refresh", request);

        //Assert
        response.IsSuccessStatusCode.Should().Be(false);
    }

    [Test]
    public async Task Refresh_NonRelatedTokens_BadRequestResult()
    {
        //Arrange
        var registerCommand2 = new RegisterUserCommand
        {
            Email = "test2@example.com",
            Password = "password@#12P2",
            FirstName = "Alex",
            Username = "@Alex222",
            LastName = "Makedonsky",
            PhoneNumber = "+992423456711",
            ConfirmPassword = "password@#12P2",
            ApplicationId = _application.Id,
            CallBackUrl = _application.CallbackUrls.First()
        };
        var res = await _client.GetAsync($"api/sms/send_code?PhoneNumber={registerCommand2.PhoneNumber}");
        res.EnsureSuccessStatusCode();

        registerCommand2.VerificationCode =
            (await GetEntity<ConfirmationCode>(x => x.PhoneNumber == registerCommand2.PhoneNumber)).Code;
        var loginCommand2 = new LoginUserCommand
        {
            Username = "@Alex222", Password = "password@#12P2",
            ApplicationId = _application.Id,
            CallBackUrl = _application.CallbackUrls.First()
        };

        await RegisterUser(registerCommand2);
        var loginResponse2 = await LoginUser(loginCommand2);

        var request = new GetAccessTokenUsingRefreshTokenQuery()
        {
            AccessToken = LoginResponse.AccessToken, RefreshToken = loginResponse2.RefreshToken
        };
        //Act
        var response = await _client.PostAsJsonAsync("api/Auth/refresh", request);

        //Assert
        response.IsSuccessStatusCode.Should().Be(false);
    }


    private async Task RegisterUser(RegisterUserCommand command)
    {
        var response = await _client.PostAsJsonAsync("api/Auth/register", command);
        response.EnsureSuccessStatusCode();
    }

    private async Task<JwtTokenResponse> LoginUser(LoginUserCommand command)
    {
        var response = await _client.PostAsJsonAsync("api/Auth/login", command);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<JwtTokenResponse>();
    }
}