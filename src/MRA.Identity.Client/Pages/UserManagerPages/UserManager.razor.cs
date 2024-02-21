﻿using Microsoft.AspNetCore.Components;
using MRA.BlazorComponents.Configuration;
using MRA.BlazorComponents.Dialogs;
using MRA.BlazorComponents.HttpClient.Services;
using MRA.Identity.Application.Contract.User.Responses;
using MudBlazor;

namespace MRA.Identity.Client.Pages.UserManagerPages;

public sealed partial class UserManager
{
    [Inject] private IHttpClientService Client { get; set; }
    [Inject] private IConfiguration Configuration { get; set; }  
    [Inject] private IDialogService DialogService { get; set; }

    private string _searchString = "";

    private IEnumerable<UserResponse> _pagedData;
    private MudTable<UserResponse> _table;

    private int _totalItems;


    private async Task<TableData<UserResponse>> ServerReload(TableState state)
    {
        IEnumerable<UserResponse> data =
            (await Client.GetFromJsonAsync<List<UserResponse>>(Configuration.GetIdentityUrl("user"))).Result;
        await Task.Delay(100);
        data = data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;
            if (element.UserName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.FullName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }).ToArray();

        _totalItems = data.Count();
        switch (state.SortLabel)
        {
            case "UserName_field":
                data = data.OrderByDirection(state.SortDirection, o => o.UserName);
                break;
            case "FullName_field":
                data = data.OrderByDirection(state.SortDirection, o => o.FullName);
                break;
            case "EmailConfirmed_field":
                data = data.OrderByDirection(state.SortDirection, o => o.EmailConfirmed);
                break;
            case "Email_field":
                data = data.OrderByDirection(state.SortDirection, o => o.Email);
                break;
            case "PhoneNumberConfirmed_field":
                data = data.OrderByDirection(state.SortDirection, o => o.PhoneNumberConfirmed);
                break;
            case "PhoneNumber_field":
                data = data.OrderByDirection(state.SortDirection, o => o.PhoneNumber);
                break;
        }

        _pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        return new TableData<UserResponse> { TotalItems = _totalItems, Items = _pagedData };
    }
    public void OpenDialog(string defaultPhoneNumber)
    {
        var parameters = new DialogParameters();
        parameters.Add("DefaultPhoneNumber", defaultPhoneNumber);
        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true };
        DialogService.Show<DialogMessageSender>("Send message", parameters, options);
    }
    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }
}