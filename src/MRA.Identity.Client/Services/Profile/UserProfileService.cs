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
using System.Net;
using Microsoft.IdentityModel.Tokens;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.BlazorComponents.Snackbar.Extensions;
using MRA.Identity.Client.Services.ContentService;
using MudBlazor;

namespace MRA.Identity.Client.Services.Profile;

public class UserProfileService(
    IHttpClientService httpClient,
    IContentService contentService,
    IConfiguration configuration,
    ISnackbar snackbar) : IUserProfileService
{
    public async Task<bool> Update(UpdateProfileCommand command)
    {
        var result = await httpClient.PutAsJsonAsync(configuration.GetIdentityUrl("Profile"), command);

        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"]);
        if (result.HttpStatusCode == HttpStatusCode.OK)
        {
            snackbar.Add(contentService["Profile:Profileupdatedsuccessfully"], Severity.Success);
            return true;
        }

        return false;
    }

    public async Task<UserProfileResponse> Get(string? userName = null)
    {
        var result = await httpClient
            .GetFromJsonAsync<UserProfileResponse>(
                configuration.GetIdentityUrl(
                    $"Profile{(userName != null ? "?userName=" + Uri.EscapeDataString(userName) : "")}"));
        return result.Result!;
    }

    public async Task<List<UserEducationResponse>?> GetEducationsByUser(string? username = null)
    {
        var uri = "Profile/GetEducationsByUser";
        if (username.IsNullOrEmpty())
            uri = $"Profile/GetEducationsByUser?username={username}";

        var result = await httpClient.GetFromJsonAsync<List<UserEducationResponse>>(configuration.GetIdentityUrl(uri));
        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"]);

        return result.Result;
    }

    public async Task<bool> CreateEducationAsync(CreateEducationDetailCommand command)
    {
        var response =
            await httpClient.PostAsJsonAsync(configuration.GetIdentityUrl("Profile/CreateEducationDetail"), command);

        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"],
            contentService["Profile:Educationdetailsadded"]);


        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> UpdateEducationAsync(UpdateEducationDetailCommand command)
    {
        var response =
            await httpClient.PutAsJsonAsync(configuration.GetIdentityUrl("Profile/UpdateEducationDetail"), command);
        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"],
            contentService["Profile:UpdateEducationsuccessfully"]);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteEducationAsync(Guid id)
    {
        var response =
            await httpClient.DeleteAsync(configuration.GetIdentityUrl($"Profile/DeleteEducationDetail/{id}"));
        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"]);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> DeleteExperienceAsync(Guid id)
    {
        var response =
            await httpClient.DeleteAsync(configuration.GetIdentityUrl($"Profile/DeleteExperienceDetail/{id}"));
        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"]);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<List<UserExperienceResponse>?> GetExperiencesByUser(string? username = null)
    {
        var uri = "Profile/GetExperiencesByUser";
        if (username.IsNullOrEmpty())
            uri = $"Profile/GetExperiencesByUser?username={username}";

        var result = await httpClient.GetFromJsonAsync<List<UserExperienceResponse>>(configuration.GetIdentityUrl(uri));
        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"]);
        return result.Result;
    }

    public async Task<bool> CreateExperienceAsync(CreateExperienceDetailCommand command)
    {
        var response =
            await httpClient.PostAsJsonAsync(configuration.GetIdentityUrl("Profile/CreateExperienceDetail"), command);
        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"],
            contentService["Profile:Experiencedetailsadded"]);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<bool> UpdateExperienceAsync(UpdateExperienceDetailCommand command)
    {
        var response =
            await httpClient.PutAsJsonAsync(configuration.GetIdentityUrl("Profile/UpdateExperienceDetail"), command);
        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"],
            contentService["Profile:UpdateEducationsuccessfully"]);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<UserSkillsResponse> GetUserSkills(string? userName = null)
    {
        var response = await httpClient
            .GetFromJsonAsync<UserSkillsResponse>(configuration.GetIdentityUrl(
                $"Profile/GetUserSkills{(userName != null ? "?userName=" + Uri.EscapeDataString(userName) : "")}"));
        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"]);
        return response.Result!;
    }

    public async Task<bool> RemoveSkillAsync(string skill)
    {
        var response =
            await httpClient.DeleteAsync(
                configuration.GetIdentityUrl($"Profile/RemoveUserSkill/{Uri.EscapeDataString(skill)}"));
        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"]);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }

    public async Task<UserSkillsResponse?> AddSkills(AddSkillsCommand command)
    {
        var response =
            await httpClient.PostAsJsonAsync<UserSkillsResponse>(configuration.GetIdentityUrl("Profile/AddSkills"),
                command);
        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"],
            contentService["Profile:AddSkillssuccessfully"]);
        return response.Result;
    }

    public async Task<bool> SendConfirmationCode(string phoneNumber)
    {
        var response =
            await httpClient.GetFromJsonAsync<bool>(
                configuration.GetIdentityUrl($"sms/send_code?PhoneNumber={phoneNumber}"));
        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"]);
        return response.Result;
    }

    public async Task<bool> CheckConfirmationCode(string phoneNumber, int? code)
    {
        var response =
            await httpClient.GetFromJsonAsync<SmsVerificationCodeStatus>(configuration.GetIdentityUrl(
                $"sms/verify_code?PhoneNumber={phoneNumber}&Code={code}"));
        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"]);
        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            if (response.Result == SmsVerificationCodeStatus.CodeVerifyFailure)
            {
                snackbar.Add(contentService["Profile:Codeisincorrector"], Severity.Error);
            }

            return true;
        }

        return false;
    }

    public async Task<List<UserEducationResponse>?> GetAllEducations()
    {
        var result = await httpClient.GetFromJsonAsync<List<UserEducationResponse>>(configuration.GetIdentityUrl("Profile/GetAllEducations"));
        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"]);

        return result.Result;
    }

    public async Task<List<UserExperienceResponse>> GetAllExperiences()
    {
        var result = await httpClient.GetFromJsonAsync<List<UserExperienceResponse>>(configuration.GetIdentityUrl("Profile/GetAllExperiences"));
        snackbar.ShowIfError(result, contentService["Profile:Servernotrespondingtry"]);
        return result.Result!;
    }

    public async Task<UserSkillsResponse> GetAllSkills()
    {
        var response = await httpClient.GetFromJsonAsync<UserSkillsResponse>(configuration.GetIdentityUrl("Profile/GetAllSkills"));
        snackbar.ShowIfError(response, contentService["Profile:Servernotrespondingtry"]);
        return response.Result!;
    }
}