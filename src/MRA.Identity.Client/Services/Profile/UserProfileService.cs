﻿using MRA.Identity.Application.Contract.Educations.Command.Create;
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

namespace MRA.Identity.Client.Services.Profile;

public class UserProfileService(HttpClient httpClient) : IUserProfileService
{
    public async Task<string> Update(UpdateProfileCommand command, IContentService ContentService)
    {
        try
        {
            var result = await httpClient.PutAsJsonAsync("Profile", command);

            if (result.IsSuccessStatusCode)
                return "";

            return result.StatusCode == HttpStatusCode.BadRequest
                ? result.RequestMessage.ToString()
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
            .GetFromJsonAsync<UserProfileResponse>(
                $"Profile{(userName != null ? "?userName=" + Uri.EscapeDataString(userName) : "")}");
        return result;
    }

    public async Task<List<UserEducationResponse>> GetEducationsByUser(string username = null)
    {
        var uri = "Profile/GetEducationsByUser";
        if (username.IsNullOrEmpty())
            uri = $"Profile/GetEducationsByUser?username={username}";

        var result = await httpClient.GetFromJsonAsync<List<UserEducationResponse>>(uri);
        return result;
    }

    public async Task<HttpResponseMessage> CreateEducationAsуnc(CreateEducationDetailCommand command)
    {
        var response = await httpClient.PostAsJsonAsync("Profile/CreateEducationDetail", command);
        return response;
    }

    public async Task<HttpResponseMessage> UpdateEducationAsync(UpdateEducationDetailCommand command)
    {
        var response = await httpClient.PutAsJsonAsync("Profile/UpdateEducationDetail", command);
        return response;
    }

    public async Task<HttpResponseMessage> DeleteEducationAsync(Guid id)
    {
        var respose = await httpClient.DeleteAsync($"Profile/DeleteEducationDetail/{id}");
        return respose;
    }

    public async Task<HttpResponseMessage> DeleteExperienceAsync(Guid id)
    {
        var respose = await httpClient.DeleteAsync($"Profile/DeleteExperienceDetail/{id}");
        return respose;
    }

    public async Task<List<UserExperienceResponse>> GetExperiencesByUser(string username = null)
    {
        var uri = "Profile/GetExperiencesByUser";
        if (username.IsNullOrEmpty())
            uri = $"Profile/GetExperiencesByUser?username={username}";

        var result = await httpClient.GetFromJsonAsync<List<UserExperienceResponse>>(uri);
        return result;
    }

    public async Task<HttpResponseMessage> CreateExperienceAsync(CreateExperienceDetailCommand command)
    {
        var response = await httpClient.PostAsJsonAsync("Profile/CreateExperienceDetail", command);
        return response;
    }

    public async Task<HttpResponseMessage> UpdateExperienceAsync(UpdateExperienceDetailCommand command)
    {
        var response = await httpClient.PutAsJsonAsync("Profile/UpdateExperienceDetail", command);
        return response;
    }

    public async Task<UserSkillsResponse> GetUserSkills(string userName = null)
    {
        var response = await httpClient
            .GetFromJsonAsync<UserSkillsResponse>(
                $"Profile/GetUserSkills{(userName != null ? "?userName=" + Uri.EscapeDataString(userName) : "")}");
        return response;
    }

    public async Task<HttpResponseMessage> RemoveSkillAsync(string skill)
    {
        var response = await httpClient.DeleteAsync($"Profile/RemoveUserSkill/{Uri.EscapeDataString(skill)}");
        return response;
    }

    public async Task<UserSkillsResponse> AddSkills(AddSkillsCommand command)
    {
        var response = await httpClient.PostAsJsonAsync("Profile/AddSkills", command);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserSkillsResponse>(content);
            return result;
        }

        return null;
    }

    public async Task<bool> SendConfirmationCode(string phoneNumber)
    {
        var response = await httpClient.GetFromJsonAsync<bool>($"sms/send_code?PhoneNumber={phoneNumber}");

        return response;
    }

    public async Task<SmsVerificationCodeStatus> CheckConfirmationCode(string phoneNumber, int? code)
    {
        var response =
            await httpClient.GetFromJsonAsync<SmsVerificationCodeStatus>(
                $"sms/verify_code?PhoneNumber={phoneNumber}&Code={code}");
        return response;
    }

    public async Task<List<UserEducationResponse>> GetAllEducations()
    {
        var result = await httpClient.GetFromJsonAsync<List<UserEducationResponse>>("Profile/GetAllEducations");
        return result;
    }

    public async Task<List<UserExperienceResponse>> GetAllExperiences()
    {
        var result = await httpClient.GetFromJsonAsync<List<UserExperienceResponse>>("Profile/GetAllExperiences");
        return result;
    }

    public async Task<UserSkillsResponse> GetAllSkills()
    {
        var response = await httpClient.GetFromJsonAsync<UserSkillsResponse>("Profile/GetAllSkills");
        return response;
    }
}