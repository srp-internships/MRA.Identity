namespace MRA.Jobs.Application.IntegrationTests.UserRoles.Queries;

public class GetUserRolesQueryTest : BaseTest
{
    [SetUp]
    public async Task SetUp()
    {
        
    }

    [Test]
    [TestCase("TestUser", HttpStatusCode.OK)]
    [TestCase("alex09991", HttpStatusCode.NotFound)]
    [Ignore("")]
    public async Task GetUserRolesQuery_HttpStatusCode(string userName, HttpStatusCode statusCode)
    {
        var user = new ApplicationUser
        {
            UserName = "TestUser",
            Email = "Test@con.ty",
        };

        await AddEntity(user);

        var role = new ApplicationRole
        {
            Slug = "rol_test",
            Name = "Rol_Test"
        };

        await AddEntity(role);

        var slug = $"{user.UserName.ToLower()}-{role.Slug}";

        var userRole = new ApplicationUserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            Slug = slug
        };
        await AddEntity(userRole);
        
        await AddAuthorizationAsync();
        var response = await _client.GetAsync(
                $"api/UserRoles?userName={userName}");
        Assert.That(response.StatusCode == statusCode);
    }
}