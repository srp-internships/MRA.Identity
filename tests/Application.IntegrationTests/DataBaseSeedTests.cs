using FluentAssertions;
using MRA.Identity.Domain.Entities;
using MRA.Identity.Infrastructure.Identity;
using MRA.Configurations.Common.Constants;

namespace MRA.Jobs.Application.IntegrationTests;

public class DataBaseSeedTests : BaseTest
{
    #region SuperAdmin
    
    private async Task<ApplicationUser> GetSuperAdmin() =>
    await GetEntity<ApplicationUser>(s => s.UserName == "SuperAdmin");
    
    [Test]
    public async Task SuperAdminRole()
    {
        var superAdminRole = await GetEntity<ApplicationRole>(s =>
            s.NormalizedName == ApplicationClaimValues.SuperAdministrator.ToUpper());
        superAdminRole.Should().NotBeNull();
    }
    
    [Test]
    public async Task SuperAdminUser()
    {
        (await GetSuperAdmin()).Should().NotBeNull();
    }
    
    [Test]
    public async Task SuperAdminClaims()
    {
        var superAdmin = await GetSuperAdmin();
    
        var roleClaim = await GetEntity<ApplicationUserClaim>(s =>
            s.UserId == superAdmin.Id &&
            s.ClaimType == ClaimTypes.Role);
    
        roleClaim.Should().NotBeNull();
    }
    
    #endregion
    
    #region MraJobsAdmin
    
    private async Task<ApplicationUser> GetMraJobsAdmin() =>
        await GetEntity<ApplicationUser>(s => s.UserName == "MraJobsAdmin");
    
    [Test]
    public async Task MraJobsAdminRole()
    {
        var superAdmin = await GetMraJobsAdmin();
        var superAdminRole = await GetEntity<ApplicationRole>(s =>
            s.NormalizedName == ApplicationClaimValues.Administrator.ToUpper());
        superAdminRole.Should().NotBeNull();
    
        var userRole = await GetEntity<ApplicationUserRole>(s =>
            s.RoleId == superAdminRole.Id &&
            s.UserId == superAdmin.Id);
    
        userRole.Should().NotBeNull();
    }
    
    [Test]
    public async Task MraJobsAdminUser()
    {
        (await GetMraJobsAdmin()).Should().NotBeNull();
    }
    
    #endregion
    
    #region MraAcademyAdmin

    private async Task<ApplicationUser> GetMraAcademyAdmin() =>
        await GetEntity<ApplicationUser>(s => s.UserName == "MraAcademyAdmin");

    [Test]
    public async Task MraAcademyRole()
    {
        var superAdmin = await GetMraAcademyAdmin();
        var superAdminRole = await GetEntity<ApplicationRole>(s =>
            s.NormalizedName == ApplicationClaimValues.Administrator.ToUpper());
        superAdminRole.Should().NotBeNull();

        var userRole = await GetEntity<ApplicationUserRole>(s =>
            s.RoleId == superAdminRole.Id &&
            s.UserId == superAdmin.Id);

        userRole.Should().NotBeNull();
    }

    [Test]
    public async Task MraAcademyAdminUser()
    {
        (await GetMraAcademyAdmin()).Should().NotBeNull();
    }
    #endregion
}