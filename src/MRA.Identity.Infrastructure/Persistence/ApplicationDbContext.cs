using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Domain.Entities;
using MRA.Identity.Infrastructure.Persistence.TableConfigurations;

namespace MRA.Identity.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        ApplicationUserClaim, ApplicationUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>(options), IApplicationDbContext
{
    public DbSet<ConfirmationCode> ConfirmationCodes { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<EducationDetail> Educations { get; set; }
    public DbSet<ExperienceDetail> Experiences { get; set; }

    public DbSet<Message> Messages { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }
    public DbSet<UserSkill> UserSkills { get; set; }
    public DbSet<Domain.Entities.Application> Applications { get; set; }
    public DbSet<ApplicationUserLink> ApplicationUserLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new ApplicationRoleConfiguration());
        builder.ApplyConfiguration(new UserSkillConfiguration());
    }
}