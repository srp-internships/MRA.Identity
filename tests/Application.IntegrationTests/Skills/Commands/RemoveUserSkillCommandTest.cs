using MRA.Identity.Application.Contract.Skills.Command;

namespace MRA.Jobs.Application.IntegrationTests.Skills.Commands;
public class RemoveUserSkillCommandTest : BaseTest
{
    [Test]
    public async Task RemoveUserSkill_ShouldRemoveUserSkill_Success()
    {
        await AddApplicantAuthorizationAsync();

        var skill = new UserSkill()
        {
            Skill = new Skill { Name = "Skill3" },
            UserId = Applicant.Id,
        };
        await AddEntity(skill);

        var command = new RemoveUserSkillCommand()
        {
            Skill = "Skill3"
        };
       

        var response = await _client.DeleteAsync($"/api/Profile/RemoveUserSkill/{command.Skill}");

        response.EnsureSuccessStatusCode();
    }
}
