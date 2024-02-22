using MRA.Configurations.Services;
using MRA.Identity.Application.Contract.UserEmail.Commands;

namespace MRA.Jobs.Application.IntegrationTests.UserEmail.Commands;

public class SendEmailCommandTests : BaseTest
{
    [Test]
    public async Task _ValidRequest_ShouldSendEmail_ReturnsOk()
    {
        var sendEmailCommand = new SendEmailCommand
        {
            Receivers = ["test@email.com"],
            Subject = "this is a subject",
            Text = "this is a text of email"
        };
        await AddAdminAuthorizationAsync();
        var postResponse = await _client.PostAsJsonAsync("api/user/sendEmail", sendEmailCommand);
        postResponse.EnsureSuccessStatusCode();
        Assert.That(SendEmailData.Receivers.First(), Is.EqualTo(sendEmailCommand.Receivers.First()));
        Assert.That(SendEmailData.Subject, Is.EqualTo(sendEmailCommand.Subject));
        Assert.That(SendEmailData.Body, Is.EqualTo(sendEmailCommand.Text));
    }
}