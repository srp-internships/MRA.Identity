﻿namespace MRA.Jobs.Application.IntegrationTests.UserProfile.Commands;

public class GeneratePdfCvTest : BaseTest
{
    [Test]
    public async Task GeneratePdfCv_ShouldReturnFile_Success()
    {
        await AddApplicantAuthorizationAsync();
        var response = await _client.GetAsync("/api/Profile/GenerateCV");
        var content = await response.Content.ReadAsByteArrayAsync();
        Assert.That(content.Length > 0, "File not received");
    }

    [Test]
    public async Task GeneratePdfCv_ShouldReturnFile_Unauthorized()
    {
        var response = await _client.GetAsync("/api/Profile/GenerateCV");
        Assert.That(HttpStatusCode.Unauthorized == response.StatusCode);
    }
}