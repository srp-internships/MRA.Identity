using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MRA.Identity.Application.Contract.ContentService;

namespace MRA.Identity.Application.Contract;

public static class Configuration
{
    public static void AddFluentValidatorCustomMessages(this IServiceCollection services)
    {
        
        services.AddScoped<IContentService, ContentService.ContentService>();
       
    }
}