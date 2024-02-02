using Microsoft.AspNetCore.Components;
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
using MRA.Identity.Client.Services.Auth;
using MudBlazor;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Client.Components.Dialogs;
using Blazored.FluentValidation;

namespace MRA.Identity.Client.Pages.Auth;

public partial class Profile
{
    [Inject] private IAuthService AuthService { get; set; }

    private bool _tryButton;
    private bool _codeSent;
    private int? _confirmationCode;
    private bool _isPhoneNumberNull = true;

    private int _active;

    private void ActiveNavLink(int active)
    {
        if (_profile != null)
        {
            _active = active;
            return;
        }

        Snackbar.Add(ContentService["Profile:Servernotrespondingtry"], Severity.Error);
    }

    private UserProfileResponse _profile;
    private readonly UpdateProfileCommand _updateProfileCommand = new();
    private FluentValidationValidator _fluentValidationValidator;

    private async Task SendCode()
    {
        var response = await UserProfileService.SendConfirmationCode(_profile.PhoneNumber);
        if (response) _codeSent = true;
    }

    private async Task SendEmailConfirms()
    {
        await AuthService.ResendVerificationEmail();
        StateHasChanged();
    }

    private async Task Verify()
    {
        var response =
            await UserProfileService.CheckConfirmationCode(_profile.PhoneNumber, _confirmationCode);

        if (response)
        {
            _profile = await UserProfileService.Get();
            _codeSent = false;
            StateHasChanged();
        }
    }

    protected override async void OnInitialized()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (!(user.Identity?.IsAuthenticated ?? true))
        {
            Navigation.NavigateTo("login");
            return;
        }

        await Load();
    }

    private async Task Load()
    {
        _tryButton = false;
        StateHasChanged();


        var profile = await UserProfileService.Get();
        if (profile == null)
        {
            return;
        }

        _profile = profile;

        _updateProfileCommand.AboutMyself = _profile.AboutMyself;
        _updateProfileCommand.DateOfBirth = _profile.DateOfBirth;
        _updateProfileCommand.Email = _profile.Email;
        _updateProfileCommand.FirstName = _profile.FirstName;
        _updateProfileCommand.LastName = _profile.LastName;
        _updateProfileCommand.Gender = _profile.Gender;
        _updateProfileCommand.PhoneNumber = _profile.PhoneNumber;
        _isPhoneNumberNull = !_updateProfileCommand.PhoneNumber.IsNullOrEmpty();


        await GetEducations();

        await GetExperiences();

        await GetSkills();

        StateHasChanged();
    }

    // private void ServerNotResponding()
    // {
    //     Snackbar.Add(ContentService["Profile:Servernotrespondingtry"], Severity.Error);
    // }

    // private async Task BadRequestResponse(HttpResponseMessage response)
    // {
    //     var customProblemDetails = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();
    //     if (customProblemDetails.Detail != null)
    //     {
    //         Snackbar.Add(customProblemDetails.Detail, Severity.Error);
    //     }
    //     else
    //     {
    //         var errorResponseString = await response.Content.ReadAsStringAsync();
    //         var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorResponseString);
    //         foreach (var error in errorResponse.Errors)
    //         {
    //             var errorMessage = string.Join(", ", error.Value);
    //             Snackbar.Add(errorMessage, Severity.Error);
    //         }
    //     }
    // }

    private async Task ConfirmDelete<T>(IList<T> collection, T item)
    {
        var parameters = new DialogParameters<DialogMudBlazor>
        {
            { x => x.ContentText, ContentService["Profile:Doyoureally"] },
            { x => x.ButtonText, ContentService["Profile:Delete"] },
            { x => x.Color, Color.Error }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

        var dialog =
            await DialogService.ShowAsync<DialogMudBlazor>(ContentService["Profile:Delete"], parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            Delete(collection, item);
        }
    }

    private async void Delete<T>(ICollection<T> collection, T item)
    {
        if (collection.Contains(item))
        {
            Type itemType = item.GetType();
            var idProperty = itemType.GetProperty("Id");
            var result = true;
            if (idProperty != null || itemType.Name == "String")
            {
                if (itemType.Name == "String")
                {
                    result = await UserProfileService.RemoveSkillAsync(item.ToString());
                }
                else
                {
                    Guid id = (Guid)idProperty?.GetValue(item)!;

                    switch (itemType.Name)
                    {
                        case "UserEducationResponse":
                            result = await UserProfileService.DeleteEducationAsync(id);
                            break;
                        case "UserExperienceResponse":
                            result = await UserProfileService.DeleteExperienceAsync(id);
                            break;
                    }
                }


                if (!result) return;
                collection.Remove(item);
                StateHasChanged();
            }
        }
    }

    private async void UpdateProfile()
    {
        var result = await UserProfileService.Update(_updateProfileCommand);
        if (result)
        {
            _profile = await UserProfileService.Get();
        }
    }

    #region education

    private List<UserEducationResponse> _educations = [];
    private bool _addEducation;
    private CreateEducationDetailCommand _createEducation = new();
    private List<UserEducationResponse> _allEducations = [];


    private Guid _editingCardId;
    private UpdateEducationDetailCommand _educationUpdate;

    private async Task GetEducations()
    {
        _educations = await UserProfileService.GetEducationsByUser() ?? default;
        _allEducations = await UserProfileService.GetAllEducations() ?? default;
    }

    private async Task<IEnumerable<string>> SearchEducation(string value)
    {
        await Task.Delay(5);

        return _allEducations.Where(e => e.Speciality
                .Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(e => e.Speciality).Distinct()
            .ToList();
    }

    private async Task<IEnumerable<string>> SearchUniversity(string value)
    {
        await Task.Delay(5);

        return _allEducations.Where(e => e.University
                .Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(e => e.University).Distinct()
            .ToList();
    }

    private void EditButtonClicked(Guid cardId, UserEducationResponse educationResponse)
    {
        _editingCardId = cardId;
        _educationUpdate = new UpdateEducationDetailCommand()
        {
            EndDate = educationResponse.EndDate.HasValue ? educationResponse.EndDate.Value : default(DateTime),
            StartDate =
                educationResponse.StartDate.HasValue ? educationResponse.StartDate.Value : default(DateTime),
            University = educationResponse.University,
            Speciality = educationResponse.Speciality,
            Id = educationResponse.Id,
            UntilNow = educationResponse.UntilNow
        };
    }

    private async Task CreateEducationHandle()
    {
        if (!await _fluentValidationValidator!.ValidateAsync())
            return;

        if (_createEducation.EndDate == null)
            _createEducation.UntilNow = true;

        var response = await UserProfileService.CreateEducationAsync(_createEducation);
        if (response)
        {
            _addEducation = false;
            _createEducation = new CreateEducationDetailCommand();
            await GetEducations();
            StateHasChanged();
        }
    }

    private void CancelButtonClicked_CreateEducation()
    {
        _addEducation = false;
        _createEducation = new CreateEducationDetailCommand();
    }

    private async void CancelButtonClicked_UpdateEducation()
    {
        _editingCardId = Guid.NewGuid();
        await GetEducations();
        StateHasChanged();
    }

    private async Task UpdateEducationHandle()
    {
        var result = await UserProfileService.UpdateEducationAsync(_educationUpdate);
        if (result)
        {
            _editingCardId = Guid.NewGuid();
            await GetEducations();
            StateHasChanged();
        }
    }

    #endregion

    #region Experience

    private List<UserExperienceResponse> _experiences = new();
    private bool _addExperience;
    private CreateExperienceDetailCommand _createExperience = new();
    private UpdateExperienceDetailCommand? _experienceUpdate = null;
    private List<UserExperienceResponse> _allExperiences = new();
    private Guid _editingCardExperienceId;

    private async Task GetExperiences()
    {
        _experiences = await UserProfileService.GetExperiencesByUser();
        _allExperiences = await UserProfileService.GetAllExperiences();
    }

    private async Task<IEnumerable<string>> SearchJobTitle(string value)
    {
        await Task.Delay(5);

        return _allExperiences.Where(e => e.JobTitle
                .Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(e => e.JobTitle).Distinct()
            .ToList();
    }

    private async Task<IEnumerable<string>> SearchCompanyName(string value)
    {
        await Task.Delay(5);

        return _allExperiences.Where(e => e.CompanyName
                .Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(e => e.CompanyName).Distinct()
            .ToList();
    }

    private async Task<IEnumerable<string>> SearchAddress(string value)
    {
        await Task.Delay(5);

        return _allExperiences.Where(e => e.Address
                .Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(e => e.Address).Distinct()
            .ToList();
    }

    private async Task CreateExperienceHandle()
    {
        if (!await _fluentValidationValidator!.ValidateAsync()) return;

        if (_createExperience.EndDate == null)
            _createExperience.IsCurrentJob = true;

        var response = await UserProfileService.CreateExperienceAsync(_createExperience);
        if (response)
        {
            _addExperience = false;
            _createExperience = new CreateExperienceDetailCommand();
            await GetExperiences();
        }
    }

    private async void CancelButtonClicked_CreateExperience()
    {
        _addExperience = false;
        _createExperience = new CreateExperienceDetailCommand();
        await GetExperiences();
    }

    private async void CancelButtonClicked_UpdateExperience()
    {
        _editingCardExperienceId = Guid.NewGuid();
        await GetExperiences();
        StateHasChanged();
    }

    private void EditCardExperienceButtonClicked(Guid cardExperienceId, UserExperienceResponse experienceResponse)
    {
        _editingCardExperienceId = cardExperienceId;
        _experienceUpdate = new UpdateExperienceDetailCommand()
        {
            Id = experienceResponse.Id,
            JobTitle = experienceResponse.JobTitle,
            CompanyName = experienceResponse.CompanyName,
            Address = experienceResponse.Address,
            IsCurrentJob = experienceResponse.IsCurrentJob,
            StartDate = experienceResponse.StartDate,
            EndDate = experienceResponse.EndDate
        };
    }

    private async void UpdateExperienceHandle()
    {
        var result = await UserProfileService.UpdateExperienceAsync(_experienceUpdate!);
        if (result)
        {
            _editingCardExperienceId = Guid.NewGuid();
            StateHasChanged();
        }
    }

    #endregion

    #region Skills

    private UserSkillsResponse _userSkills;
    private UserSkillsResponse _allSkills;
    private string _newSkills = "";

    private async Task GetSkills()
    {
        _userSkills = await UserProfileService.GetUserSkills();
        _allSkills = await UserProfileService.GetAllSkills();
    }

    async void Closed(MudChip chip)
    {
        await ConfirmDelete(_userSkills.Skills, chip.Text);
    }


    private async Task OnBlur()
    {
        await AddSkills();
    }

    private async Task AddSkills(string foundSkill = null)
    {
        if (foundSkill != null)
            _newSkills = foundSkill;

        if (!string.IsNullOrWhiteSpace(_newSkills))
        {
            var userSkillsCommand = new AddSkillsCommand();
            var skills = _newSkills.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var skill in skills)
            {
                var trimmedSkill = skill.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedSkill))
                {
                    userSkillsCommand.Skills.Add(trimmedSkill);
                }
            }

            var result = await UserProfileService.AddSkills(userSkillsCommand);
            if (result != null)
            {
                Snackbar.Add(ContentService["Profile:AddSkillssuccessfully"], Severity.Success);

                _newSkills = "";
                _userSkills = result;
                StateHasChanged();
            }
        }
    }

    private async Task<IEnumerable<string>> SearchSkills(string value)
    {
        await Task.Delay(5);
        var userSkillsSet = new HashSet<string>(_userSkills.Skills);
        return _allSkills.Skills
            .Where(s => s.Contains(value, StringComparison.InvariantCultureIgnoreCase) && !userSkillsSet.Contains(s))
            .ToList();
    }

    #endregion
}