﻿using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Client.Enums;
using MRA.Identity.Client.Resources;
using MRA.Identity.Client.Services.ContentService;
using MRA.Identity.Client.Services.UserPreferences;
using MudBlazor;

namespace MRA.Identity.Client.Services;

public class LayoutService(
    IUserPreferencesService userPreferencesService,
    IContentService contentService,
    IConfiguration configuration)
{
    private UserPreferences.UserPreferences _userPreferences;
    private bool _systemPreferences;

    public DarkLightMode DarkModeToggle = DarkLightMode.System;

    public bool IsDarkMode { get; private set; }

    public MudTheme CurrentTheme { get; private set; }

    public void SetDarkMode(bool value)
    {
        IsDarkMode = value;
    }

    public void SetNoTheme()
    {
        IsDarkMode = false;
        DarkModeToggle = DarkLightMode.Light;
        OnMajorUpdateOccured();
    }

    public async Task ApplyUserPreferences(bool isDarkModeDefaultTheme)
    {
        _systemPreferences = isDarkModeDefaultTheme;
        _userPreferences = await userPreferencesService.LoadUserPreferences();
        if (_userPreferences != null)
        {
            IsDarkMode = _userPreferences.DarkLightTheme switch
            {
                DarkLightMode.Dark => true,
                DarkLightMode.Light => false,
                DarkLightMode.System => isDarkModeDefaultTheme,
                _ => IsDarkMode
            };
        }
        else
        {
            IsDarkMode = isDarkModeDefaultTheme;
            _userPreferences = new UserPreferences.UserPreferences { DarkLightTheme = DarkLightMode.System };
            await userPreferencesService.SaveUserPreferences(_userPreferences);
        }

        var lang = await contentService.GetCurrentCulture();
        Lang = lang.IsNullOrEmpty() ? configuration["FeatureManagement:DefaultLanguage"] : lang;
    }

    public void OnSystemPreferenceChanged(bool newValue)
    {
        _systemPreferences = newValue;
        if (DarkModeToggle == DarkLightMode.System)
        {
            IsDarkMode = newValue;
            OnMajorUpdateOccured();
        }
    }

    public event EventHandler MajorUpdateOccured;

    private void OnMajorUpdateOccured() => MajorUpdateOccured?.Invoke(this, EventArgs.Empty);

    public async Task ToggleDarkMode()
    {
        switch (DarkModeToggle)
        {
            case DarkLightMode.System:
                DarkModeToggle = DarkLightMode.Light;
                IsDarkMode = false;
                break;
            case DarkLightMode.Light:
                DarkModeToggle = DarkLightMode.Dark;
                IsDarkMode = true;
                break;
            case DarkLightMode.Dark:
                DarkModeToggle = DarkLightMode.System;
                IsDarkMode = _systemPreferences;
                break;
        }

        _userPreferences.DarkLightTheme = DarkModeToggle;
        await userPreferencesService.SaveUserPreferences(_userPreferences);
        OnMajorUpdateOccured();
    }

    public string Lang =configuration["FeatureManagement:DefaultLanguage"];

    public async Task ChangeLanguage(string lang)
    {
        Lang = lang;

        if (lang == "Ru")
            await contentService.ChangeCulture(ApplicationCulturesNames.Ru);
        if (lang == "En")
            await contentService.ChangeCulture(ApplicationCulturesNames.En);
        if (lang == "Tj")
            await contentService.ChangeCulture(ApplicationCulturesNames.Tj);

        OnMajorUpdateOccured();
    }

    public void SetBaseTheme(MudTheme theme)
    {
        CurrentTheme = theme;
        OnMajorUpdateOccured();
    }
}