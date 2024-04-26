using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Infrastructure.Identity;

namespace MRA.Jobs.Application.IntegrationTests.Users.Query;

public class GetUserByKeyQueryTests : BaseTest
{
    private static readonly Guid Id = Guid.NewGuid();
    private static readonly string UserName = "user";

    [SetUp]
    public async Task SetUp()
    {
        await AddUser();
    }

    private static IEnumerable<string> GetTestCases()
    {
        yield return Id.ToString();
        yield return UserName;
    }

    [Test]
    [TestCaseSource(nameof(GetTestCases))]
    public async Task GetUserByKey_ReturnUserResponse(string key)
    {
        await AddAuthorizationAsync();
        var response = await _client.GetAsync($"api/User/{key}");
        response.EnsureSuccessStatusCode();

        Assert.That((await response.Content.ReadFromJsonAsync<UserResponse>()) is not null);
    }

    [Test]
    public async Task GetUserByKey_Return_NotFoundException()
    {
        await AddAuthorizationAsync();
        var response = await _client.GetAsync($"api/User/{Guid.NewGuid()}");
        Assert.That(response.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    [TestCase("", HttpStatusCode.Forbidden)]
    [TestCase(ApplicationPolicies.Reviewer, HttpStatusCode.Forbidden)]
    [TestCase(ApplicationPolicies.Administrator, HttpStatusCode.OK,
        "4f67d20a-4f2a-4c7f-8a35-4c15c2d0c3e2", "mraJobsApplicationSecret")]
    [TestCase(ApplicationPolicies.SuperAdministrator, HttpStatusCode.OK)]
    public async Task GetUserByKey_Return_StatusCode(string role, HttpStatusCode statusCode,
        string applicationId = null, string applicationClientSecret = null)
    {
        if (role == ApplicationPolicies.SuperAdministrator || role == ApplicationPolicies.Administrator)
            await AddAuthorizationAsync();
        else if (role == ApplicationPolicies.Reviewer)
            await AddReviewerAuthorizationAsync();
        else
            await AddApplicantAuthorizationAsync();

        var response =
            await _client.GetAsync(
                $"api/User/{UserName}");
        Assert.That(response.StatusCode == statusCode);
    }

    private async Task AddUser()
    {
        var user = await GetEntity<ApplicationUser>(a => a.UserName == UserName);
        if (user != null) return;
        user = new ApplicationUser()
        {
            Id = Id,
            UserName = UserName
        };
        await AddEntity(user);
    }
}