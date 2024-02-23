using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.Identity.Application.Contract.Common;
using MRA.Identity.Application.Contract.Skills.Responses;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Client.Services.Profile;
using MudBlazor;

namespace MRA.Identity.Client.Pages.UserManagerPages;

public sealed partial class UserManager
{
    [Inject] private IHttpClientService Client { get; set; }
    [Inject] private IConfiguration Configuration { get; set; }
    [Inject] private IUserProfileService UserProfileService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    private string _searchString = "";

    private GetAllUsersQueryByFilters _query = new();
    private MudTable<UserResponse> _table;
    private UserSkillsResponse _allSkills;
    private string SelectedSkills { get; set; } = "";
    private IEnumerable<string> Options { get; set; } = new HashSet<string>();


    protected override async Task OnInitializedAsync()
    {
        _allSkills = await UserProfileService.GetAllSkills();
        if (_allSkills != null)
        {
            _allSkills.Skills = _allSkills.Skills.Distinct().OrderBy(x => x).ToList();
        }

        var currentUri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

        if (currentUri.Query.IsNullOrEmpty())
        {
            _query.Page = 1;
            _query.PageSize = 10;
        }

        if (QueryHelpers.ParseQuery(currentUri.Query).TryGetValue("page", out var page))
            _query.Page = int.Parse(page);

        if (QueryHelpers.ParseQuery(currentUri.Query).TryGetValue("pageSize", out var pageSize))
            _query.PageSize = int.Parse(pageSize);

        if (QueryHelpers.ParseQuery(currentUri.Query).TryGetValue("Skills", out var skills))
        {
            _query.Skills = skills;
        }

        if (QueryHelpers.ParseQuery(currentUri.Query).TryGetValue("filters", out var filters))
        {
            _query.Filters = filters;
        }

        if (!_query.Skills.IsNullOrEmpty()) Options = _query.Skills.Split(", ").ToList();

        if (!_query.Filters.IsNullOrEmpty())
        {
            var filterParts = _query.Filters.Split("@=");
            if (filterParts.Length > 1)
            {
                _searchString = filterParts[1].Replace("|", " ");
            }
        }

        Console.WriteLine(@$"filter: {_query.PageSize} , {_query.Page}, {_query.Filters}");

        await _table.ReloadServerData();
    }

    private async Task<TableData<UserResponse>> ServerReload(TableState state)
    {
        _query.Page = state.Page + 1;
        _query.PageSize = state.PageSize;
        if (!_searchString.IsNullOrEmpty())
        {
            var searchTerms = _searchString.Replace(",", "|").Split(" ").Select(s => s.Trim());
            _query.Filters = $"(UserName|FirstName|LastName)@={string.Join("|", searchTerms)}";
        }

        if (Options != null)
        {
            _query.Skills = string.Join(",", Options.Select(x => x.Trim())).ToString();
        }

        var queryParam = HttpUtility.ParseQueryString(string.Empty);
        if (!_query.Skills.IsNullOrEmpty()) queryParam["Skills"] = _query.Skills;
        queryParam["Page"] = _query.Page.ToString();
        queryParam["PageSize"] = _query.PageSize.ToString();
        if (!_query.Filters.IsNullOrEmpty()) queryParam["Filters"] = _query.Filters;

        var result =
            (await Client.GetFromJsonAsync<PagedList<UserResponse>>(Configuration.GetIdentityUrl($"user?{queryParam}")))
            .Result;
        UpdateUri();

        return new TableData<UserResponse>()
        {
            TotalItems = result.TotalCount,
            Items = result.Items
        };
    }

    private void OnSearch(string text)
    {
        _searchString = text;
    }

    private string GetMultiSelectionText(List<string> selectedValues)
    {
        return string.Join(", ", selectedValues).Trim();
    }

    private void UpdateUri()
    {
        var queryParam = HttpUtility.ParseQueryString(string.Empty);
        if (!_query.Skills.IsNullOrEmpty()) queryParam["Skills"] = _query.Skills;
        queryParam["Page"] = _query.Page.ToString();
        queryParam["PageSize"] = _query.PageSize.ToString();
        if (!_query.Filters.IsNullOrEmpty()) queryParam["Filters"] = _query.Filters;

        var newUri = $"{NavigationManager.BaseUri}UserManager?{queryParam}";
        NavigationManager.NavigateTo(newUri, forceLoad: false);
    }
}