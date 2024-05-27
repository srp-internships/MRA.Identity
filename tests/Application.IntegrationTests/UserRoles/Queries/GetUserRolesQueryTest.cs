using Microsoft.AspNetCore.Http;

namespace MRA.Jobs.Application.IntegrationTests.UserRoles.Queries;

public class GetUserRolesQueryTest : BaseTest
{
    [Test]
    public async Task GetUserRolesQuery_HttpStatusCode()
    {
        var user = new ApplicationUser
        {
            UserName = "TestUser",
            Email = "Test@con.ty",
        };

        await AddUser(user, "adkjf!@QSFa21");   

        var role = new ApplicationRole
        {
            Slug = "rol_test",
            Name = "Rol_Test",
            NormalizedName = "ROL_TEST"
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
            $"api/UserRoles?userName={user.UserName}");
        Assert.That((int)response.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
    }
}