using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.Common;
using MRA.Identity.Application.Contract.User.Commands.UsersByApplications;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Jobs.Application.IntegrationTests.Users;

public class GetUsersByApplicationsTests : BaseTest
{
    private readonly Guid _application1Id = Guid.NewGuid();
    private readonly string _application1ClientSecret = "secret1";
    private readonly Guid _application2Id = Guid.NewGuid();
    private readonly string _application2ClientSecret = "secret1";
    private readonly Guid _user1Id = Guid.NewGuid();
    private readonly Guid _user2Id = Guid.NewGuid();

    [SetUp]
    public async Task SetUp()
    {
        await AddEntity(new Identity.Domain.Entities.Application()
        {
            Name = "Mra Jobs", Slug = "mra-jobs",
            Id = _application1Id,
            ClientSecret = _application1ClientSecret,
            IsProtected = true
        });
        await AddEntity(new Identity.Domain.Entities.Application()
            {
                Name = "Mra Academy", Slug = "mra-academy",
                Id = _application2Id,
                ClientSecret = _application2ClientSecret,
                IsProtected = true
            }
        );

        await AddEntity(new ApplicationUser()
        {
            Id = _user1Id,
            UserName = "Use1",
            FirstName = "User1FirstName",
            LastName = "User1LastName",
            Email = "user1@gmail.com"
        });
        await AddEntity(new ApplicationUser()
        {
            Id = _user2Id,
            UserName = "Use2",
            FirstName = "User2FirstName",
            LastName = "User2LastName",
            Email = "user2@gmail.com"
        });

        await AddEntity(new ApplicationUserLink()
        {
            UserId = _user1Id,
            ApplicationId = _application1Id
        });
        await AddEntity(new ApplicationUserLink()
        {
            UserId = _user2Id,
            ApplicationId = _application2Id
        });
    }

    [Test]
    public async Task GetPagedListUsers1()
    {
        var command = new GetPagedListUsersCommand
        {
            ApplicationId = _application1Id,
            ApplicationClientSecret = _application1ClientSecret
        };
        var response = await _client.PostAsJsonAsync("api/User", command);
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<PagedList<UserResponse>>();
        Assert.That(users.Items.Count == 1);
    }
}