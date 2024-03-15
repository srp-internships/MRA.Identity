using Microsoft.AspNetCore.Identity;
using MRA.Identity.Domain.Enumes;
using Sieve.Attributes;

namespace MRA.Identity.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    [Sieve(CanFilter = true, CanSort = true)]
    public override string UserName { get; set; }

    public Gender Gender { get; set; }
    
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime DateOfBirth { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string FirstName { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string LastName { get; set; }

    public string AboutMyself { get; set; }

    public ICollection<EducationDetail> Educations { get; set; }
    public ICollection<ExperienceDetail> Experiences { get; set; }
    public IEnumerable<UserSkill> UserSkills { get; set; }
}