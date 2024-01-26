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
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Client.Services.ContentService;
using MRA.Identity.Client.Services.HttpClientService;

namespace MRA.Identity.Client.Services.Profile;

public class UserProfileService(IHttpClientService httpClient ,IContentService ContentService) : IUserProfileService
{
    public async Task<string> Update(UpdateProfileCommand command)
    {
        try
        {
            var result = await httpClient.PutAsJsonAsync<bool>("Profile", command);

            if (result.Success)
                return "";

            return result.HttpStatusCode == HttpStatusCode.BadRequest
                ? result.Error
                : ContentService["Profile:Servernotrespondingtry"];
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            return ContentService["Profile:Servernotrespondingtry"];
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return ContentService["Profile:Anerroroccurred"];
        }
    }

    public async Task<UserProfileResponse> Get(string userName = null)
    {
        var result = await httpClient
            .GetAsJsonAsync<UserProfileResponse>(
                $"Profile{(userName != null ? "?userName=" + Uri.EscapeDataString(userName) : "")}");
        return result.Result;
    }

    public async Task<List<UserEducationResponse>> GetEducationsByUser(string username = null)
    {
        var uri = "Profile/GetEducationsByUser";
        if (username.IsNullOrEmpty())
            uri = $"Profile/GetEducationsByUser?username={username}";

        var result = await httpClient.GetAsJsonAsync<List<UserEducationResponse>>(uri);
        return result.Result;
    }

    public async Task<ApiResponse> CreateEducationAsуnc(CreateEducationDetailCommand command)
    {
        var response = await httpClient.PostAsJsonAsync<Guid>("Profile/CreateEducationDetail", command);
        return response;
    }

    public async Task<ApiResponse> UpdateEducationAsync(UpdateEducationDetailCommand command)
    {
        var response = await httpClient.PutAsJsonAsync<Guid>("Profile/UpdateEducationDetail", command);
        return response;
    }

    public async Task<ApiResponse> DeleteEducationAsync(Guid id)
    {
        var respose = await httpClient.DeleteAsync($"Profile/DeleteEducationDetail/{id}");
        return respose;
    }

    public async Task<ApiResponse> DeleteExperienceAsync(Guid id)
    {
        var respose = await httpClient.DeleteAsync($"Profile/DeleteExperienceDetail/{id}");
        return respose;
    }

    public async Task<List<UserExperienceResponse>> GetExperiencesByUser(string username = null)
    {
        var uri = "Profile/GetExperiencesByUser";
        if (username.IsNullOrEmpty())
            uri = $"Profile/GetExperiencesByUser?username={username}";

        var result = await httpClient.GetAsJsonAsync<List<UserExperienceResponse>>(uri);
        return result.Result;
    }

    public async Task<ApiResponse> CreateExperienceAsync(CreateExperienceDetailCommand command)
    {
        var response = await httpClient.PostAsJsonAsync<Guid>("Profile/CreateExperienceDetail", command);
        return response;
    }

    public async Task<ApiResponse> UpdateExperienceAsync(UpdateExperienceDetailCommand command)
    {
        var response = await httpClient.PutAsJsonAsync<Guid>("Profile/UpdateExperienceDetail", command);
        return response;
    }

    public async Task<UserSkillsResponse> GetUserSkills(string userName = null)
    {
        var response = await httpClient
            .GetAsJsonAsync<UserSkillsResponse>(
                $"Profile/GetUserSkills{(userName != null ? "?userName=" + Uri.EscapeDataString(userName) : "")}");
        return response.Result;
    }

    public async Task<ApiResponse> RemoveSkillAsync(string skill)
    {
        var response = await httpClient.DeleteAsync($"Profile/RemoveUserSkill/{Uri.EscapeDataString(skill)}");
        return response;
    }

    public async Task<UserSkillsResponse> AddSkills(AddSkillsCommand command)
    {
        var response = await httpClient.PostAsJsonAsync<UserSkillsResponse>("Profile/AddSkills", command);
        if (response.Success)
        {
            return response.Result;
        }

        return null;
    }

    public async Task<bool> SendConfirmationCode(string phoneNumber)
    {
        var response = await httpClient.GetAsJsonAsync<bool>($"sms/send_code?PhoneNumber={phoneNumber}");
        return response.Result;
    }

    public async Task<SmsVerificationCodeStatus> CheckConfirmationCode(string phoneNumber, int? code)
    {
        var response =
            await httpClient.GetAsJsonAsync<SmsVerificationCodeStatus>(
                $"sms/verify_code?PhoneNumber={phoneNumber}&Code={code}");
        return response.Result;
    }

    public async Task<List<UserEducationResponse>> GetAllEducations()
    {
        var result = await httpClient.GetAsJsonAsync<List<UserEducationResponse>>("Profile/GetAllEducations");
        return result.Result;
    }

    public async Task<List<UserExperienceResponse>> GetAllExperiences()
    {
        var result = await httpClient.GetAsJsonAsync<List<UserExperienceResponse>>("Profile/GetAllExperiences");
        return result.Result;
    }

    public async Task<UserSkillsResponse> GetAllSkills()
    {
        var response = await httpClient.GetAsJsonAsync<UserSkillsResponse>("Profile/GetAllSkills");
        return response.Result;
    }
}