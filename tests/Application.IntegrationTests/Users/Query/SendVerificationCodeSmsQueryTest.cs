using MRA.Identity.Application.Contract.User.Queries;
using MRA.Jobs.Application.IntegrationTests.Services;

namespace MRA.Jobs.Application.IntegrationTests.Users.Query;

[TestFixture]
public class SendVerificationCodeSmsQueryTest : BaseTest
{
    [Test]
    public async Task SendVerificationCodeSmsQueryHandler_SendingSmsWithCorrectPhoneNumber_Success()
    {
        // Arrange
        await AddAuthorizationAsync();
        var query = new SendVerificationCodeSmsQuery { PhoneNumber = "911111111" };

        // Act
        var response = await _client.GetAsync($"api/sms/send_code?PhoneNumber={query.PhoneNumber}");
        response.EnsureSuccessStatusCode();

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.That("true" == responseContent, "Expected response content to be 'true'.");
            Assert.That(TestSmsSandbox.PhoneNumber, Is.Not.Null);
            Assert.That(TestSmsSandbox.Text, Is.Not.Null);
        });
    }

    [Test]
    public async Task SendVerificationCodeSmsQueryHandler_SendingSmsWithCorrectPhoneNumberAndDifferentFormat_Success()
    {
        // Arrange
        await AddAuthorizationAsync();
        var query = new SendVerificationCodeSmsQuery { PhoneNumber = "992911111111" };

        // Act
        var response = await _client.GetAsync($"api/sms/send_code?PhoneNumber={query.PhoneNumber}");
        response.EnsureSuccessStatusCode();

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.That("true" == responseContent, "Expected response content to be 'true'.");
            Assert.That(TestSmsSandbox.PhoneNumber, Is.Not.Null);
            Assert.That(TestSmsSandbox.Text, Is.Not.Null);
        });
    }

    [Test]
    public async Task SendVerificationCodeSmsQueryHandler_SendingSmsWithCorrectPhoneNumberAndAnotherFormat_Success()
    {
        // Arrange
        await AddAuthorizationAsync();
        var query = new SendVerificationCodeSmsQuery { PhoneNumber = "+992911111111" };

        // Act
        var response = await _client.GetAsync($"api/sms/send_code?PhoneNumber={query.PhoneNumber}");
        response.EnsureSuccessStatusCode();

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.That("true" == responseContent, "Expected response content to be 'true'.");
            Assert.That(TestSmsSandbox.PhoneNumber, Is.Not.Null);
            Assert.That(TestSmsSandbox.Text, Is.Not.Null);
        });
    }

    [Test]
    public async Task SmsVerificationCodeCheckQueryHandler_VerifyingSmsCode_Success()
    {
        // Arrange
        await AddAuthorizationAsync();
        var query = new SendVerificationCodeSmsQuery { PhoneNumber = "+992123456789" };
        await _client.GetAsync($"api/sms/send_code?PhoneNumber={query.PhoneNumber}");

        int code = (await GetEntity<ConfirmationCode>(c => c.PhoneNumber == query.PhoneNumber)).Code;

        // Act
        var response = await _client.GetAsync($"api/sms/verify_code?PhoneNumber={query.PhoneNumber}&Code={code}");
        response.EnsureSuccessStatusCode();

        // Assert
        var responseContent = await response.Content.ReadFromJsonAsync<SmsVerificationCodeStatus>();

        Assert.That(SmsVerificationCodeStatus.CodeVerifySuccess == responseContent,
            "Expected response content to be 'true'.");
    }
}