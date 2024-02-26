using System.Reflection;
using MRA.Identity.Application;
using MRA.Identity.Infrastructure;
using MRA.Identity.Infrastructure.Persistence;
using MRA.Identity.Api.Filters;
using FluentValidation;
using MRA.Identity.Application.Contract.Skills.Command;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using MRA.Configurations.Initializer.Azure.Insight;
using MRA.Configurations.Initializer.Azure.KeyVault;
using MRA.Configurations.Initializer.Azure.AppConfig;
using Sieve.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.ConfigureAzureKeyVault("MRAIdentity");
    string appConfigConnectionString = builder.Configuration["AppConfigConnectionString"];
    builder.Configuration.AddAzureAppConfig(appConfigConnectionString);
    builder.Logging.AddApiApplicationInsights(builder.Configuration);
}

builder.Services.AddControllers(options => { options.Filters.Add<ApiExceptionFilterAttribute>(); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo());
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(typeof(RemoveUserSkillCommand).Assembly);
builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("MraIdentity-Sieve"));

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbMigration>();
    await initializer.InitialiseAsync();
}

var applicationDbContextInitializer = app.Services.CreateAsyncScope().ServiceProvider
    .GetRequiredService<ApplicationDbContextInitializer>();
await applicationDbContextInitializer.SeedAsync();

app.UseCors("CORS_POLICY");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();