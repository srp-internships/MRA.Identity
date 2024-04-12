using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Domain.Entities;
using MRA.Identity.Infrastructure.Identity;
using MRA.Configurations.Common.Constants;
using MRA.Identity.Application.Common.Interfaces.Services;
using Newtonsoft.Json;

namespace MRA.Identity.Infrastructure.Persistence;

public class ApplicationDbContextInitializer(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IApplicationDbContext context,
    IConfiguration configuration,
    ICryptoStringService cryptoStringService)
{
    private ApplicationRole _superAdminRole = null!;
    private ApplicationRole _applicationRole = null!;

    public async Task SeedAsync()
    {
        await CreateRolesAsync();

        await CreateSuperAdminAsync();

        await CreateApplicationAdmin("MraJobs", "12345678");
        await CreateApplicationAdmin("MraOnlinePlatform", "12345678");
        await CreateApplicationsAsync();

        if (configuration["Environment"] != "Production")
        {
            await CreateSeedUsersAsync();
            await CreateSeedExperiencesEducationsSkillsAsync();

            await context.SaveChangesAsync();
        }
    }

    private async Task CreateApplicationsAsync()
    {
        var mraJobsApplicationSecret = "mraJobsApplicationSecret";
        var mraAssetsManagementSecret = "mraJobsApplicationSecret";
        if (configuration["Environment"] == "Production")
        {
            mraJobsApplicationSecret = cryptoStringService.GetCryptoString();
            mraAssetsManagementSecret = cryptoStringService.GetCryptoString();
        }

        if (await context.Applications.FirstOrDefaultAsync(x => x.Name == "Mra Jobs") == null)
            await context.Applications.AddAsync(
                new Domain.Entities.Application()
                {
                    Id = Guid.Parse("4f67d20a-4f2a-4c7f-8a35-4c15c2d0c3e2"),
                    Name = "Mra Jobs",
                    Slug = "mra-jobs",
                    IsProtected = false,
                    ClientSecret = mraJobsApplicationSecret,
                    Description = "",
                    DefaultRoleId = (await roleManager.FindByNameAsync("Applicant"))!.Id,
                    CallbackUrls = ["https://localhost:7071", "https://staging.jobs.srp.tj", "https://jobs.srp.tj"]
                });
        if (await context.Applications.FirstOrDefaultAsync(x => x.Name == "MRA Assets Management") == null)
            await context.Applications.AddAsync(
                new Domain.Entities.Application()
                {
                    Id = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8"),
                    Name = "MRA Assets Management",
                    Slug = "mra-assets-management",
                    IsProtected = true,
                    Description = "",
                    DefaultRoleId = (await roleManager.FindByNameAsync("Reviewer"))!.Id,
                    CallbackUrls = [],
                    ClientSecret = mraAssetsManagementSecret
                });
        await context.SaveChangesAsync();
    }


    private async Task CreateApplicationAdmin(string applicationName, string adminPassword)
    {
        //create user
        var mraJobsAdminUser =
            await userManager.Users.SingleOrDefaultAsync(u =>
                u.NormalizedUserName == $"{applicationName}ADMIN".ToUpper());

        if (mraJobsAdminUser == null)
        {
            mraJobsAdminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = $"{applicationName}Admin",
                NormalizedUserName = $"{applicationName}ADMIN".ToUpper(),
                Email = $"{applicationName.ToLower()}admin@silkroadprofessionals.com",
            };

            var createMraJobsAdminResult = await userManager.CreateAsync(mraJobsAdminUser, adminPassword);
            ThrowExceptionFromIdentityResult(createMraJobsAdminResult);
        }

        var application = await context.Applications.FirstOrDefaultAsync(x => x.Slug == "mra-jobs");
        if (application != null)
        {
            if (await context.ApplicationUserLinks.FirstOrDefaultAsync(x =>
                    x.ApplicationId == application.Id && x.UserId == mraJobsAdminUser.Id) == null)
            {
                await context.ApplicationUserLinks.AddAsync(new ApplicationUserLink()
                {
                    ApplicationId = application.Id,
                    UserId = mraJobsAdminUser.Id
                });
            }
        }

        //create user

        //create userRole
        var userRole = new ApplicationUserRole
        {
            UserId = mraJobsAdminUser.Id,
            RoleId = _applicationRole.Id,
            Slug = $"{mraJobsAdminUser.UserName}-role"
        };

        if (!await context.UserRoles.AnyAsync(s => s.RoleId == userRole.RoleId && s.UserId == userRole.UserId))
        {
            await context.UserRoles.AddAsync(userRole);
            await context.SaveChangesAsync();
        }
        //create userRole

        //create role claim
        if (!await context.UserClaims.AnyAsync(s =>
                s.UserId == mraJobsAdminUser.Id &&
                s.ClaimType == ClaimTypes.Role))
        {
            var userRoleClaim = new ApplicationUserClaim
            {
                UserId = mraJobsAdminUser.Id,
                ClaimType = ClaimTypes.Role,
                ClaimValue = ApplicationClaimValues.Administrator,
                Slug = $"{mraJobsAdminUser.UserName}-role"
            };
            await context.UserClaims.AddAsync(userRoleClaim);
        }
        //create role claim

        //create email claim
        if (!await context.UserClaims.AnyAsync(s =>
                s.UserId == mraJobsAdminUser.Id &&
                s.ClaimType == ClaimTypes.Role))
        {
            var userRoleClaim = new ApplicationUserClaim
            {
                UserId = mraJobsAdminUser.Id,
                ClaimType = ClaimTypes.Email,
                ClaimValue = mraJobsAdminUser.Email,
                Slug = $"{mraJobsAdminUser.UserName}-email"
            };
            await context.UserClaims.AddAsync(userRoleClaim);
        }
        //create email claim


        //create application claim
        if (!await context.UserClaims.AnyAsync(s =>
                s.UserId == mraJobsAdminUser.Id &&
                s.ClaimType == ClaimTypes.Application))
        {
            var userApplicationClaim = new ApplicationUserClaim
            {
                UserId = mraJobsAdminUser.Id,
                ClaimType = ClaimTypes.Application,
                ClaimValue = applicationName,
                Slug = $"{mraJobsAdminUser.UserName}-application"
            };
            await context.UserClaims.AddAsync(userApplicationClaim);
        }

        //create application claim

        //create username claim
        if (!await context.UserClaims.AnyAsync(s =>
                s.UserId == mraJobsAdminUser.Id &&
                s.ClaimType == ClaimTypes.Username))
        {
            var userApplicationClaim = new ApplicationUserClaim
            {
                UserId = mraJobsAdminUser.Id,
                ClaimType = ClaimTypes.Username,
                ClaimValue = mraJobsAdminUser.UserName,
                Slug = $"{mraJobsAdminUser.UserName}-username"
            };
            await context.UserClaims.AddAsync(userApplicationClaim);
        }
        //create username claim

        //create id claim
        if (!await context.UserClaims.AnyAsync(s =>
                s.UserId == mraJobsAdminUser.Id &&
                s.ClaimType == ClaimTypes.Id))
        {
            var userApplicationClaim = new ApplicationUserClaim
            {
                UserId = mraJobsAdminUser.Id,
                ClaimType = ClaimTypes.Id,
                ClaimValue = mraJobsAdminUser.Id.ToString(),
                Slug = $"{mraJobsAdminUser.UserName}-id"
            };
            await context.UserClaims.AddAsync(userApplicationClaim);
        }
        //create id claim

        await context.SaveChangesAsync();
    }

    private async Task CreateSuperAdminAsync()
    {
        var superAdmin = await userManager.Users.SingleOrDefaultAsync(s => s.NormalizedUserName == "SUPERADMIN");
        if (superAdmin == null)
        {
            superAdmin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "SuperAdmin",
                NormalizedUserName = "SUPERADMIN",
                Email = "mraidentity@silkroadprofessionals.com",
                NormalizedEmail = "mraidentity@silkroadprofessionals.com",
                EmailConfirmed = false,
            };
            var superAdminResult = await userManager.CreateAsync(superAdmin, "Mra123!!@#$AGfer4");
            ThrowExceptionFromIdentityResult(superAdminResult);

            var userRole = new ApplicationUserRole
            {
                UserId = superAdmin.Id,
                RoleId = _superAdminRole.Id,
                Slug = $"role-{superAdmin.UserName}"
            };

            if (!await context.UserRoles.AnyAsync(s => s.RoleId == userRole.RoleId && s.UserId == userRole.UserId))
            {
                await context.UserRoles.AddAsync(userRole);
                await context.SaveChangesAsync();
            }

            //create role claim
            var claim = new ApplicationUserClaim
            {
                ClaimType = ClaimTypes.Role,
                ClaimValue = _superAdminRole.Name,
                Slug = $"{superAdmin.UserName}-role",
                UserId = superAdmin.Id
            };
            await context.UserClaims.AddAsync(claim);
            //create role claim

            //create application claim
            claim = new ApplicationUserClaim
            {
                ClaimType = ClaimTypes.Application,
                ClaimValue = ApplicationClaimValues.AllApplications,
                Slug = $"{superAdmin.UserName}-application",
                UserId = superAdmin.Id
            };
            await context.UserClaims.AddAsync(claim);
            //create application claim

            //create username claim
            claim = new ApplicationUserClaim
            {
                ClaimType = ClaimTypes.Username,
                ClaimValue = superAdmin.UserName,
                Slug = $"{superAdmin.UserName}-username",
                UserId = superAdmin.Id
            };
            await context.UserClaims.AddAsync(claim);
            //create username claim

            //create email claims
            claim = new ApplicationUserClaim
            {
                ClaimType = ClaimTypes.Email,
                ClaimValue = superAdmin.Email,
                Slug = $"{superAdmin.UserName}-email",
                UserId = superAdmin.Id
            };
            await context.UserClaims.AddAsync(claim);
            //create email claim

            //create id claim
            claim = new ApplicationUserClaim
            {
                ClaimType = ClaimTypes.Id,
                ClaimValue = superAdmin.Id.ToString(),
                Slug = $"{superAdmin.UserName}-id",
                UserId = superAdmin.Id
            };
            await context.UserClaims.AddAsync(claim);
            //create id claim

            await context.SaveChangesAsync();
        }
    }

    private async Task CreateRolesAsync()
    {
        //superAdmin
        IdentityResult createRoleResult;
        if (!await context.Roles.AnyAsync(s => s.Slug == ApplicationClaimValues.SuperAdministrator))
        {
            var roleSuper = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = ApplicationClaimValues.SuperAdministrator,
                NormalizedName = ApplicationClaimValues.SuperAdministrator.ToUpper(),
                Slug = ApplicationClaimValues.SuperAdministrator
            };
            createRoleResult = await roleManager.CreateAsync(roleSuper);
            ThrowExceptionFromIdentityResult(createRoleResult);
            _superAdminRole = roleSuper;
        } //superAdmin

        //applicationAdmin
        _applicationRole =
            await context.Roles.SingleOrDefaultAsync(s => s.Slug == ApplicationClaimValues.Administrator);
        if (_applicationRole == null)
        {
            _applicationRole = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = ApplicationClaimValues.Administrator,
                NormalizedName = ApplicationClaimValues.Administrator.ToUpper(),
                Slug = ApplicationClaimValues.Administrator
            };
            createRoleResult = await roleManager.CreateAsync(_applicationRole);
            ThrowExceptionFromIdentityResult(createRoleResult);
        }
        //applicationAdmin

        ApplicationRole role;

        //reviewer
        var roleString = "Reviewer";
        if (!await context.Roles.AnyAsync(s => s.Slug == roleString))
        {
            role = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = roleString,
                NormalizedName = roleString.ToUpper(),
                Slug = roleString
            };

            createRoleResult = await roleManager.CreateAsync(role);
            ThrowExceptionFromIdentityResult(createRoleResult);
        }
        //reviewer

        //applicant
        roleString = "Applicant";
        if (!await context.Roles.AnyAsync(s => s.Slug == roleString))
        {
            role = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = roleString,
                NormalizedName = roleString.ToUpper(),
                Slug = roleString
            };

            createRoleResult = await roleManager.CreateAsync(role);
            ThrowExceptionFromIdentityResult(createRoleResult);
        }
        //applicant

        //teacher
        roleString = "Teacher";
        if (!await context.Roles.AnyAsync(s => s.Slug == roleString))
        {
            role = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = roleString,
                NormalizedName = roleString.ToUpper(),
                Slug = roleString
            };

            createRoleResult = await roleManager.CreateAsync(role);
            ThrowExceptionFromIdentityResult(createRoleResult);
        } //teacher

        //student
        roleString = "Student";
        if (!await context.Roles.AnyAsync(s => s.Slug == roleString))
        {
            role = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = roleString,
                NormalizedName = roleString.ToUpper(),
                Slug = roleString
            };

            createRoleResult = await roleManager.CreateAsync(role);
            ThrowExceptionFromIdentityResult(createRoleResult);
        }
        //student
    }

    private class UserModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPhoneVerificationRequired { get; set; }
        public bool IsEmailVerificationRequired { get; set; }
        public int VerificationCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    private async Task CreateSeedUsersAsync()
    {
        string json = @"[
{
    'Email': 'durdona@silkroadprofessionals.com',
    'FirstName': 'Durdona',
    'LastName': 'Uzoqova',
    'PhoneNumber': '+992113456789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'durdona',
    'Password': 'RandomPassword1',
    'ConfirmPassword': 'RandomPassword1'
  },
  {
    'Email': 'niazovd@gmail.com',
    'FirstName': 'Doniyor',
    'LastName': 'Niyozov',
    'PhoneNumber': '+992123156789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 5555,
    'Username': 'doniyor',
    'Password': 'RandomPassword2',
    'ConfirmPassword': 'RandomPassword2'
  },
  {
    'Email': 'zubaidullo.nematov@silkroadprofessionals.com',
    'FirstName': 'Zubaidullo',
    'LastName': 'Nematov',
    'PhoneNumber': '+992123423789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'zubaidullo',
    'Password': 'RandomPassword3',
    'ConfirmPassword': 'RandomPassword3'
  },{
    'Email': 'alisa.esh@silkroadprofessionals.com',
    'FirstName': 'Alisa',
    'LastName': 'Esh',
    'PhoneNumber': '+992123326789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'alisa',
    'Password': 'RandomPassword4',
    'ConfirmPassword': 'RandomPassword4'
  },
  {
    'Email': 'abbosk@silkroadprofessionals.com',
    'FirstName': 'Abbos',
    'LastName': 'Kamolov',
    'PhoneNumber': '+992123346789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'abbos',
    'Password': 'RandomPassword5',
    'ConfirmPassword': 'RandomPassword5'
  },
  {
    'Email': 'rahmatillo.abduqodirov@silkroadprofessionals.com',
    'FirstName': 'Rahmatillo',
    'LastName': 'Abduqodirov',
    'PhoneNumber': '+992123443789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'rahmatillo',
    'Password': 'RandomPassword6',
    'ConfirmPassword': 'RandomPassword6'
  },
  {
    'Email': 'parvina@silkroadprofessionals.com',
    'FirstName': 'Parvina',
    'LastName': 'Zulfikorova',
    'PhoneNumber': '+992123246789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'parvina',
    'Password': 'RandomPassword7',
    'ConfirmPassword': 'RandomPassword7'
  },{
    'Email': 'ghanijon.safarov@silkroadprofessionals.com',
    'FirstName': 'Ghanijon',
    'LastName': 'Safarov',
    'PhoneNumber': '+992126556789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'ghanijon',
    'Password': 'RandomPassword8',
    'ConfirmPassword': 'RandomPassword8'
  },
  {
    'Email': 'mirolim@silkroadprofessionals.com',
    'FirstName': 'Mirolim',
    'LastName': 'Majidov',
    'PhoneNumber': '+992123676789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'mirolim',
    'Password': 'RandomPassword9',
    'ConfirmPassword': 'RandomPassword9'
  },
  {
    'Email': 'muhammadislom.ismatov@silkroadprofessionals.com',
    'FirstName': 'Muhammadislom',
    'LastName': 'Ismatov',
    'PhoneNumber': '+992123876789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'muhammadislom',
    'Password': 'RandomPassword10',
    'ConfirmPassword': 'RandomPassword10'
  },
  {
    'Email': 'abbosidiqov@silkroadprofessionals.com',
    'FirstName': 'Abbos',
    'LastName': 'Sidiqov',
    'PhoneNumber': '+992723456789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'abbosidiqov',
    'Password': 'RandomPassword11',
    'ConfirmPassword': 'RandomPassword11'
  },{
    'Email': 'bohir@silkroadprofessionals.com',
    'FirstName': 'Bohirjon',
    'LastName': 'Ahmedov',
    'PhoneNumber': '+992123776789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'bohirjon',
    'Password': 'RandomPassword11',
    'ConfirmPassword': 'RandomPassword11'
  },
  {
    'Email': 'usmonjonn@silkroadprofessionals.com',
    'FirstName': 'Usmonjon',
    'LastName': 'Nurmatov',
    'PhoneNumber': '+992653456789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'usmonjon',
    'Password': 'RandomPassword12',
    'ConfirmPassword': 'RandomPassword12'
  },
  {
    'Email': 'tursunhujan@silkroadprofessionals.com',
    'FirstName': 'Tursunhuja',
    'LastName': 'Norhujaev',
    'PhoneNumber': '+992126456789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'tursunhuja',
    'Password': 'RandomPassword13',
    'ConfirmPassword': 'RandomPassword13'
  },
  {
    'Email': 'sanjar@silkroadprofessionals.com',
    'FirstName': 'Sanjar',
    'LastName': 'Akhmedov',
    'PhoneNumber': '+992123656789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'sanjar',
    'Password': 'RandomPassword14',
    'ConfirmPassword': 'RandomPassword14'
  },
  {
    'Email': 'islomjon.makhsudov@silkroadprofessionals.com',
    'FirstName': 'Islomjon',
    'LastName': 'Makhsudov',
    'PhoneNumber': '+992123456589',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'islomjon',
    'Password': 'RandomPassword15',
    'ConfirmPassword': 'RandomPassword15'
  },
  {
    'Email': 'mirzodalerm@silkroadprofessionals.com',
    'FirstName': 'Mirzodaler',
    'LastName': 'Muhsinzoda',
    'PhoneNumber': '+992123456989',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'mirzodaler',
    'Password': 'RandomPassword16',
    'ConfirmPassword': 'RandomPassword16'
  },
{
    'Email': 'shuhrat@silkroadprofessionals.com',
    'FirstName': 'Shuhrat',
    'LastName': 'Rahmonov',
    'PhoneNumber': '+992123459789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'shuhrat',
    'Password': 'RandomPassword17',
    'ConfirmPassword': 'RandomPassword17'
  },
  {
    'Email': 'samuel.rivera@silkroadprofessionals.com',
    'FirstName': 'Samuel',
    'LastName': 'Rivera',
    'PhoneNumber': '+992123456669',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'samuel',
    'Password': 'RandomPassword18',
    'ConfirmPassword': 'RandomPassword18'
  },
  {
    'Email': 'malika@silkroadprofessionals.com',
    'FirstName': 'Malika',
    'LastName': 'Mukhsinova',
    'PhoneNumber': '+992123465789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'malika',
    'Password': 'RandomPassword19',
    'ConfirmPassword': 'RandomPassword19'
  },
  {
    'Email': 'nizomjon@silkroadprofessionals.com',
    'FirstName': 'Nizomjon',
    'LastName': 'Rahmonberdiev',
    'PhoneNumber': '+992553456789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'nizomjon',
    'Password': 'RandomPassword20',
    'ConfirmPassword': 'RandomPassword20'
  },
  {
    'Email': 'o.khakimova@silkroadprofessionals.com',
    'FirstName': 'Olga',
    'LastName': 'Khakimova',
    'PhoneNumber': '+992773456789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'o.khakimova',
    'Password': 'RandomPassword21',
    'ConfirmPassword': 'RandomPassword21'
  },
  {
    'Email': 'abdurasul@silkroadprofessionals.com',
    'FirstName': 'Abdurasul',
    'LastName': 'Isoqov',
    'PhoneNumber': '+992177456789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'abdurasul',
    'Password': 'RandomPassword22',
    'ConfirmPassword': 'RandomPassword22'
  },
  {
    'Email': 'komiljon.najmitdinov@silkroadprofessionals.com',
    'FirstName': 'Komiljon',
    'LastName': 'Najmitdinov',
    'PhoneNumber': '+992123477789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'komiljon',
    'Password': 'RandomPassword23',
    'ConfirmPassword': 'RandomPassword23'
  },
  {
    'Email': 'azimjon.faizulloev@silkroadprofessionals.com',
    'FirstName': 'Azimjon',
    'LastName': 'Faizulloev',
    'PhoneNumber': '+992123456779',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'azimjon',
    'Password': 'RandomPassword24',
    'ConfirmPassword': 'RandomPassword24'
  },{
    'Email': 'doniyor@silkroadprofessionals.com',
    'FirstName': 'Doniyor',
    'LastName': 'Niyozov',
    'PhoneNumber': '+992123456777',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'doniyor',
    'Password': 'RandomPassword25',
    'ConfirmPassword': 'RandomPassword25'
  },
  {
    'Email': 'dalerjon.olimov@silkroadprofessionals.com',
    'FirstName': 'Dalerjon',
    'LastName': 'Olimov',
    'PhoneNumber': '+992123450089',
    'IsPhoneVerificationRequired': false,   
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'dalerjon',
    'Password': 'RandomPassword26',
    'ConfirmPassword': 'RandomPassword26'
  },
  {
    'Email': 'muhammad@silkroadprofessionals.com',
    'FirstName': 'Muhammad',
    'LastName': 'Abdugafforov',
    'PhoneNumber': '+992100456789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'muhammad',
    'Password': 'RandomPassword27',
    'ConfirmPassword': 'RandomPassword27'
  },
  {
    'Email': 'orasta@silkroadprofessionals.com',
    'FirstName': 'Orasta',
    'LastName': 'Mirdadoeva',
    'PhoneNumber': '+992983456789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'orasta',
    'Password': 'RandomPassword28',
    'ConfirmPassword': 'RandomPassword28'
  },
  {
    'Email': 'asliya@silkroadprofessionals.com',
    'FirstName': 'Asliya',
    'LastName': 'Boturova',
    'PhoneNumber': '+992993456789',
    'IsPhoneVerificationRequired': false,
    'IsEmailVerificationRequired': false,
    'VerificationCode': 1234,
    'Username': 'asliya',
    'Password': 'RandomPassword29',
    'ConfirmPassword': 'RandomPassword29'
  }]";
        var users = JsonConvert.DeserializeObject<List<UserModel>>(json);
        var mraAssetsManagement =
            await context.Applications.FirstOrDefaultAsync(x => x.Name == "MRA Assets Management");
        foreach (var user in users)
        {
            await CreateTestUser(user.Username, user.FirstName, user.LastName, user.Email, user.Password,
                mraAssetsManagement);
        }

        var mraJobs = await context.Applications.FirstOrDefaultAsync(x => x.Slug == "mra-jobs");

        await CreateTestUser("applicant1", "ApplicantTest", "ApplicantTest", "applicant1@gmail.com",
            "applicantPassword", mraJobs);

        await CreateTestUser("Jerry", "Tom", "Jerry", "tom1234@gmail.com",
            "tom1234", mraJobs);
    }

    private async Task CreateTestUser(string username, string firstname, string lastname, string email,
        string password, Domain.Entities.Application application = null)
    {
        if (await userManager.FindByNameAsync(username) != null) return;

        ApplicationUser user = new()
        {
            UserName = username.Trim(),
            NormalizedUserName = username.Trim().ToLower(),
            Email = email,
            NormalizedEmail = email.ToLower(),
            EmailConfirmed = false,
            FirstName = firstname,
            LastName = lastname,
        };

        await userManager.CreateAsync(user, password);
        var newUser = await userManager.FindByNameAsync(user.UserName);
        if (newUser != null)
        {
            await CreateClaimAsync(newUser.UserName, newUser.Id, newUser.Email);
            if (application != null)
            {
                await context.ApplicationUserLinks.AddAsync(new ApplicationUserLink()
                {
                    ApplicationId = application.Id,
                    UserId = newUser.Id
                });
            }
        }
    }

    private async Task CreateSeedExperiencesEducationsSkillsAsync()
    {
        var user = await userManager.Users
            .Include(u => u.Educations)
            .Include(u => u.Experiences)
            .Include(u => u.UserSkills)
            .FirstOrDefaultAsync(x => x.UserName == "applicant1");
        if (user == null) return;

        if (user.Educations.Count == 0)
        {
            EducationDetail education = new()
            {
                University = "Таджикский Национальный Университет",
                Speciality = "Компьютерные науки",
                StartDate = new DateTime(2018, 9, 1),
                EndDate = new DateTime(2022, 6, 30),
                UserId = user.Id
            };
            await context.Educations.AddAsync(education);
        }

        if (user.Experiences.Count == 0)
        {
            ExperienceDetail experience = new()
            {
                JobTitle = "Старший разработчик",
                CompanyName = "ООО 'Инновации'",
                Description =
                    "Разработка и поддержка веб-приложений на C#. Работа в команде, участие в планировании и оценке задач.",
                StartDate = new DateTime(2022, 7, 1),
                EndDate = new DateTime(2024, 3, 29),
                Address = "ул. Ленина, 10, Душанбе, Таджикистан",
                UserId = user.Id
            };
            await context.Experiences.AddAsync(experience);
        }

        if (!user.UserSkills.Any())
        {
            List<UserSkill> userSkills =
            [
                new UserSkill { Skill = new Skill() { Name = "dotNet" }, UserId = user.Id },
                new UserSkill { Skill = new Skill() { Name = "scrum" }, UserId = user.Id }
            ];
            await context.UserSkills.AddRangeAsync(userSkills);
        }

        await context.SaveChangesAsync();
    }

    private async Task CreateClaimAsync(string username, Guid id, string email,
        CancellationToken cancellationToken = default)
    {
        var userClaims = new[]
        {
            new ApplicationUserClaim
            {
                UserId = id, ClaimType = ClaimTypes.Id, ClaimValue = id.ToString(), Slug = $"{username}-id"
            },
            new ApplicationUserClaim
            {
                UserId = id,
                ClaimType = ClaimTypes.Username,
                ClaimValue = username,
                Slug = $"{username}-username"
            },
            new ApplicationUserClaim
            {
                UserId = id, ClaimType = ClaimTypes.Email, ClaimValue = email, Slug = $"{username}-email"
            }
        };
        await context.UserClaims.AddRangeAsync(userClaims, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static void ThrowExceptionFromIdentityResult(IdentityResult result,
        [System.Runtime.CompilerServices.CallerLineNumber]
        int sourceLineNumber = 0)
    {
        if (!result.Succeeded)
        {
            throw new Exception(
                $"{result.Errors.First().Description}\n exception was thrown at line {sourceLineNumber}");
        }
    }
}