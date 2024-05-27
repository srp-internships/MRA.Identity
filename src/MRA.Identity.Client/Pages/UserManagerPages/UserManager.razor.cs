using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.Identity.Application.Contract.Applications.Responses;
using MRA.Identity.Application.Contract.Common;
using MRA.Identity.Application.Contract.Skills.Responses;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Client.Services.Applications;
using MRA.Identity.Client.Services.Profile;
using MudBlazor;

namespace MRA.Identity.Client.Pages.UserManagerPages;

public sealed partial class UserManager
{
    [Inject] private IHttpClientService Client { get; set; }
    [Inject] private IUserProfileService UserProfileService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IConfiguration Configuration { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private IApplicationsService ApplicationsService { get; set; }

    private string SelectedApplications { get; set; } = "";
    private List<ApplicationResponse> _applications;
    private bool _isLoaded = false;
    private string _searchString = "";

    private GetPagedListUsersQuery _query = new();
    private MudTable<UserResponse> _table;
    private IEnumerable<string> Options { get; set; } = new HashSet<string>();


    protected override async Task OnInitializedAsync()
    {
        _applications = await ApplicationsService.GetAllAsync();
        StateHasChanged();
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

        if (QueryHelpers.ParseQuery(currentUri.Query).TryGetValue("ApplicationIds", out var applicationIds))
        {
            Options = new HashSet<string>();
            var list = applicationIds.Select(id => _applications.FirstOrDefault(x => x.Id == Guid.Parse(id)))
                .Where(application => application != null).Select(application => application.Name).ToList();
            Options = list;
        }

        if (QueryHelpers.ParseQuery(currentUri.Query).TryGetValue("filters", out var filters))
        {
            var filterParts = filters.ToString().Split("@=");
            if (filterParts.Length > 1)
            {
                _searchString = filterParts[1].Replace("|", " ");
            }
        }

        StateHasChanged();
        _isLoaded = true;
        StateHasChanged();
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
        else
        {
            _query.Filters = "";
        }

        if (Options != null)
        {
            var applications = SelectedApplications.Split(",");
            var applicationIds = _applications
                .Where(x => applications.Contains(x.Name))
                .Select(x => x.Id)
                .ToList();

            _query.ApplicationIds = string.Join(",", applicationIds);
        }

        var queryParam = HttpUtility.ParseQueryString(string.Empty);
        if (!_query.ApplicationIds.IsNullOrEmpty()) queryParam["ApplicationIds"] = _query.ApplicationIds;
        queryParam["Page"] = _query.Page.ToString();
        queryParam["PageSize"] = _query.PageSize.ToString();
        if (!_query.Filters.IsNullOrEmpty()) queryParam["Filters"] = _query.Filters;

        UpdateUri();

        var response =
            await Client.GetFromJsonAsync<PagedList<UserResponse>>(Configuration.GetIdentityUrl($"user?{queryParam}"));
        if (!response.Success) return new TableData<UserResponse>();

        var result = response.Result;
        return new TableData<UserResponse>()
        {
            TotalItems = result.TotalCount,
            Items = result.Items
        };
    }

    private string GetMultiSelectionText(List<string> selectedValues)
    {
        return string.Join(", ", selectedValues).Trim();
    }

    private void UpdateUri()
    {
        var queryParam = HttpUtility.ParseQueryString(string.Empty);
        if (!_query.ApplicationIds.IsNullOrEmpty()) queryParam["ApplicationIds"] = _query.ApplicationIds;
        queryParam["Page"] = _query.Page.ToString();
        queryParam["PageSize"] = _query.PageSize.ToString();
        if (!_query.Filters.IsNullOrEmpty()) queryParam["Filters"] = _query.Filters;
        var newUri = $"{NavigationManager.BaseUri}UserManager?{queryParam}";
        NavigationManager.NavigateTo(newUri, forceLoad: false);
    }
}