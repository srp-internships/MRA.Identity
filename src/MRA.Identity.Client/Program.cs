using System.Net.Mime;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FeatureManagement;
using MRA.BlazorComponents;
using MRA.BlazorComponents.Dialogs;
using MRA.BlazorComponents.DynamicPages;
using MRA.BlazorComponents.HttpClient;
using MRA.Identity.Application.Contract;
using MRA.Identity.Client;
using MRA.Identity.Client.Services;
using MRA.Identity.Client.Services.Applications;
using MRA.Identity.Client.Services.Auth;
using MRA.Identity.Client.Services.Profile;
using MRA.Identity.Client.Services.Roles;
using MRA.Identity.Client.Services.UserPreferences;
using MudBlazor.Services;
using ContentService = MRA.Identity.Client.Services.ContentService.ContentService;
using IContentService = MRA.Identity.Client.Services.ContentService.IContentService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices();

builder.Services.AddFeatureManagement();

//Mra.BlazorComponents
builder.Services.AddHttpClientService();
builder.Services.AddMraPages();
builder.Services.AddDialogs();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
//Mra.BlazorComponents

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IApplicationsService, ApplicationsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<LayoutService>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<IUserPreferencesService, UserPreferencesService>();
builder.Services.AddScoped<ITokenParserService, TokenParserService>();
builder.Services.AddLocalization();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddFluentValidatorCustomMessages();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();