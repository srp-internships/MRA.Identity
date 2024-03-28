using MRA.Identity.Application.Contract.Applications.Responses;

namespace MRA.Jobs.Application.IntegrationTests.Applications.Queries;

public class GetApplicationsTests : BaseTest
{
    private readonly List<Identity.Domain.Entities.Application> _applications = [];

    [SetUp]
    public async Task SetUp()
    {
        await AddAuthorizationAsync();
        for (var i = 0; i < 10; i++)
        {
            var application = new Identity.Domain.Entities.Application
            {
                Name = $"Name {i}",
                Description = $"Description {i}",
                ClientSecret = $"ClientSecret {i}",
                CallbackUrls =
                [
                    $"http://localhost:2013/{i}",
                    $"http://localhost:2013/{i + 10}"
                ],
                IsProtected = i % 2 == 0,
                Slug = $"slug{i}"
            };
            await AddEntity(application);
            _applications.Add(application);
        }
    }

    [Test]
    public async Task _Client_GetAll_ReturnAllEntities()
    {
        var response = await _client.GetAsync("api/applications");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<ApplicationResponse>>();
        result.Should().NotBeNull();
        result.Count.Should().BeGreaterThanOrEqualTo(_applications.Count);

        for (var i = 0; i < 10; i++)
        {
            result.Any(s => s.Name == _applications[i].Name).Should().BeTrue();
            result.Any(s => s.Description == _applications[i].Description).Should().BeTrue();
            result.Any(s => s.ClientSecret == _applications[i].ClientSecret).Should().BeTrue();
            result.Any(s => s.CallbackUrls.SequenceEqual(_applications[i].CallbackUrls)).Should().BeTrue();
            result.Any(s => s.IsProtected == _applications[i].IsProtected).Should().BeTrue();
        }
    }
}