using MRA.Configurations.Services;
using MRA.Identity.Application.Contract.User.Commands.RegisterUser;

namespace MRA.Jobs.Application.IntegrationTests.Email;

[TestFixture]
public class EmailTest : BaseTest
{
    [Test]
    [Ignore("emailTemplate")]
    public async Task Email_VerifyEmail_True()
    {
        // Arrange
        var request = new RegisterUserCommand
        {
            Email = "test1@example.com",
            Password = "password@#12P",
            FirstName = "Alex",
            Username = "@Alex221",
            LastName = "Makedonsky1",
            PhoneNumber = "+992323456789",
            ConfirmPassword = "password@#12P"
        };
        var res = await _client.GetAsync($"api/sms/send_code?PhoneNumber={request.PhoneNumber}");
        res.EnsureSuccessStatusCode();

        request.VerificationCode = (await GetEntity<ConfirmationCode>(x => x.PhoneNumber == request.PhoneNumber)).Code;

        var response = await _client.PostAsJsonAsync("api/Auth/register", request);
        var splitted = SendEmailData.Body.Split("token=")[1].Split("&userId=");
        var token = splitted[0];
        var userId = splitted.Length > 1 ? splitted[1].Split("'>Ссылка</a>")[0] : string.Empty;

        var responseEmail = await _client.GetAsync($"api/Auth/verify?token={token}&userId={userId}");

        var stringres = responseEmail.Content.ReadAsStringAsync();

        // Assert

        responseEmail.EnsureSuccessStatusCode();
        var user = await GetEntity<ApplicationUser>(s => s.Email == request.Email);
        Assert.That(user.EmailConfirmed);
    }

    [Test]
    public async Task Email_VerifyEmail_BadRequest()
    {
        // Arrange
        var request = new RegisterUserCommand
        {
            Email = "test1@example.com",
            Password = "password@#12P",
            FirstName = "Alex",
            Username = "@Alex221",
            LastName = "Makedonsky1",
            PhoneNumber = "+992323456789",
            ConfirmPassword = "password@#12P"
        };
        // Act
        var res = await _client.GetAsync($"api/sms/send_code?PhoneNumber={request.PhoneNumber}");
        res.EnsureSuccessStatusCode();

        request.VerificationCode = (await GetEntity<ConfirmationCode>(x => x.PhoneNumber == request.PhoneNumber)).Code;
        var response = await _client.PostAsJsonAsync("api/Auth/register", request);
        var userId = (await response.Content.ReadAsStringAsync()).Replace("\"", "");
        var splitted = "aaaa1";

        var responseEmail =
            await _client.GetAsync($"api/Auth/verify?token={WebUtility.UrlEncode(splitted)}&userId={userId}");
        // Assert

        Assert.That(responseEmail.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var user = await GetEntity<ApplicationUser>(s => s.Email == request.Email);
        Assert.That(!user.EmailConfirmed);
    }
}