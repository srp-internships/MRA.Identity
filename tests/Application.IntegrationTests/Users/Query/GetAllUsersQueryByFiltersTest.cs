using MRA.Identity.Application.Contract.Common;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Jobs.Application.IntegrationTests.Users.Query;

public class GetAllUsersQueryByFiltersTest : BaseTest
{
    [SetUp]
    public async Task SetUp()
    {
        var user = new ApplicationUser()
        {
            UserName = "testBySkills",
            FirstName = "Test",
            LastName = "test",
            Email = "testbySkills@gmail.com",
            PhoneNumber = "+9921001010",
            UserSkills =
            [
                new UserSkill { Skill = new Skill() { Name = "UserSkillTest1" } },
                new UserSkill { Skill = new Skill() { Name = "UserSkillTest2" } }
            ]
        };
        await AddUser(user, "helloWorld@pass123");
    }

    [Test]
    [TestCase("UserSkillTest1")]
    [TestCase("UserSkillTest2")]
    [TestCase("UserSkillTest1,UserSkillTest2")]
    [Ignore("")]
    public async Task GetAllUsersQuery_FilterBySkills_returnListUsers(string skills)
    {
        await AddAdminAuthorizationAsync();
        var url = $"api/User?Skills={skills}";
        var response = await _client.GetFromJsonAsync<PagedList<UserResponse>>(url);

        Assert.That(response.Items.Count == 1);
    }
}