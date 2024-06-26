﻿using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Domain.Entities;
using MRA.Identity.Infrastructure.Account.Services;
using MRA.Identity.Infrastructure.Identity;
using MRA.Identity.Infrastructure.Persistence;
using MRA.Configurations.Initializer.Azure.EmailService;
using MRA.Configurations.Initializer.Services;
using MRA.Configurations.Initializer.OsonSms.SmsService;
using MRA.Configurations.Common.Constants;
using MRA.Identity.Application.Contract;
using MRA.Identity.Infrastructure.Services;

namespace MRA.Identity.Infrastructure;

public static class DependencyInitializer
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configurations)
    {
        services.AddTransient<ISlugService, SlugService>();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            string dbConnectionString = configurations.GetConnectionString("DefaultConnection");
            if (configurations["UseInMemoryDatabase"] == "true")
                options.UseInMemoryDatabase("testDB");
            else
                options.UseSqlServer(dbConnectionString);
        });

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<ApplicationDbContextInitializer>();

        services.AddScoped<DbMigration>();
        services.AddHttpClient();

        services.AddScoped<IEmailVerification, EmailVerification>();

        services.AddScoped<IUserHttpContextAccessor, UserHttpContextAccessor>();

        if (configurations["UseFileEmailService"] == "true")
        {
            services.AddFileEmailService();
        }
        else
        {
            services.AddAzureEmailService(); //uncomment this if u wont use email service from Azure from namespace MRA.Configurations.Initializer.Azure.EmailService
        }

        if (configurations["UseFileSmsService"] == "true")
        {
            services.AddFileSmsService();
        }
        else
        {
            services.AddOsonSmsService();
        }

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.ClaimsIdentity.EmailClaimType = ClaimTypes.Email;
                options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
                options.ClaimsIdentity.UserIdClaimType = ClaimTypes.Id;
                options.ClaimsIdentity.UserNameClaimType = ClaimTypes.Username;
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();


        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(op =>
        {
            op.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configurations["JWT:Secret"])),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization(auth =>
        {
            auth.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            auth.AddPolicy(ApplicationPolicies.SuperAdministrator, op => op
                .RequireRole(ApplicationClaimValues.SuperAdministrator));

            auth.AddPolicy(ApplicationPolicies.Administrator, op => op
                .RequireRole(ApplicationClaimValues.SuperAdministrator, ApplicationClaimValues.Administrator));

            auth.AddPolicy(ApplicationPolicies.Reviewer, op => op
                .RequireRole(ApplicationClaimValues.Reviewer, ApplicationClaimValues.Administrator,
                    ApplicationClaimValues.SuperAdministrator));
        });

        var corsAllowedHosts = configurations.GetSection("MraIdentity-CORS").Get<string[]>();
        services.AddCors(options =>
        {
            options.AddPolicy("CORS_POLICY", policyConfig =>
            {
                policyConfig.WithOrigins(corsAllowedHosts)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
}