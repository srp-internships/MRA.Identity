namespace MRA.Identity.Client.Enums;

public enum DocPages
{
    Home,
    Jobs,
    Internships,
    Trainings,
    Team,
    Academy,
    Contact,
    Profile,
    Applications,
    NoVacancyUploadCv
}

    public static string GetPageFromUrl(string url, bool setDefault = false)
    {
        var allPages = typeof(DocPages).GetFields().Select(s => s.GetValue(null) as string);
        var result = allPages.FirstOrDefault(s => url.EndsWith(s, StringComparison.OrdinalIgnoreCase) || url.EndsWith($"dashboard/{s}", StringComparison.OrdinalIgnoreCase));
        if (setDefault && result == null)
            result = Profile;
        return result;
    }
}