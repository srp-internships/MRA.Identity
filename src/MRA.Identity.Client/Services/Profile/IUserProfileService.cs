#nullable enable
using MRA.Identity.Application.Contract.Educations.Command.Create;
using MRA.Identity.Application.Contract.Educations.Command.Update;
using MRA.Identity.Application.Contract.Educations.Responses;
using MRA.Identity.Application.Contract.Experiences.Commands.Create;
using MRA.Identity.Application.Contract.Experiences.Commands.Update;
using MRA.Identity.Application.Contract.Experiences.Responses;
using MRA.Identity.Application.Contract.Profile.Commands.UpdateProfile;
using MRA.Identity.Application.Contract.Profile.Responses;
using MRA.Identity.Application.Contract.Skills.Command;
using MRA.Identity.Application.Contract.Skills.Responses;
using MRA.Identity.Application.Contract.User.Queries;

namespace MRA.Identity.Client.Services.Profile;

public interface IUserProfileService
{
    Task<UserProfileResponse> Get(string? userName = null);
    Task<bool> Update(UpdateProfileCommand command);

    Task<List<UserEducationResponse>?> GetEducationsByUser(string? username=null);
    Task<List<UserEducationResponse>?> GetAllEducations();

    Task<bool> CreateEducationAsync(CreateEducationDetailCommand command);

    Task<bool> UpdateEducationAsync(UpdateEducationDetailCommand command);
    Task<bool> DeleteEducationAsync(Guid id);
    Task<bool> DeleteExperienceAsync(Guid id);

    Task<List<UserExperienceResponse>?> GetExperiencesByUser(string? username=null);
    Task<List<UserExperienceResponse>> GetAllExperiences();

    Task<bool> CreateExperienceAsync(CreateExperienceDetailCommand command);

    Task<bool> UpdateExperienceAsync(UpdateExperienceDetailCommand command);

    Task<UserSkillsResponse> GetUserSkills(string? userName = null);
    Task<UserSkillsResponse> GetAllSkills();
    Task<bool> RemoveSkillAsync(string skill);

    Task<UserSkillsResponse?> AddSkills(AddSkillsCommand command);

    Task<bool> SendConfirmationCode(string phoneNumber);
    Task<bool> CheckConfirmationCode(string phoneNumber, int? code);
}
