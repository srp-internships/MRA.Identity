using MRA.Identity.Application.Contract.User.Responses;
using QuestPDF.Helpers;

namespace MRA.Jobs.Application.IntegrationTests.Users.Query;

public class GetListUsersQueryTests : BaseTest
{
    [SetUp]
    public async Task SetUp()
    {
        var user = new ApplicationUser()
        {
            UserName = "ben",
            FirstName = "Ben",
            LastName = "Denison",
            Email = "denison@gmail.com",
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
    public async Task GetListUsersQueryTests_Filters_ReturnListUsers()
    {
        await AddAdminAuthorizationAsync();
        var url =
            $"api/User/GetListUsers/ByFilter?FullName=Ben Denison&Email=denison@gmail.com&PhoneNumber=+9921001010&Skills=UserSkillTest2";
        var response = await _client.GetFromJsonAsync<List<UserResponse>>(url);

        Assert.That(response.Count == 1);
    }
}