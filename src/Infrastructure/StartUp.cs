using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Application;
using Application.Features.Identity.Tokens;
using Application.Wrappers;
using Finbuckle.MultiTenant;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Infrastructure.Indentity.Auth;
using Infrastructure.Indentity.Models;
using Infrastructure.Indentity.Tokens;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Infrastructure;

public static class StartUp
{
    public static IServiceCollection AddMInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<TenantDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionSqlServer")))
            .AddMultiTenant<ABCSchoolTenantInfo>()
                .WithHeaderStrategy(TenancyConstants.TenantIdName)
                .WithClaimStrategy(TenancyConstants.TenantIdName)
                .WithEFCoreStore<TenantDbContext, ABCSchoolTenantInfo>()
                .Services
            .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionSqlServer")))
            .AddTransient<ITenantDbSeeder, TenantDbSeeder>()
            .AddTransient<ApplicationDbSeeder>()
            .AddIdentityServices()
            .AddPermissions();
    }

    public static async Task AddDatabaseInitializerAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var tenantDbSeeder = scope.ServiceProvider.GetRequiredService<ITenantDbSeeder>();
        await tenantDbSeeder.InitializeDatabaseAsync(cancellationToken);
    }

    internal static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        // Identity services configuration can be added here in the future
        return services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .Services
            .AddScoped<ITokenService, TokenService>();
    }

    internal static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
    }

    public static JwtSettings GetJwtSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettingsConfig = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettingsConfig);

        return jwtSettingsConfig.Get<JwtSettings>()!;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
    {
        var secret = Encoding.ASCII.GetBytes(jwtSettings.Secret);

        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(bearer =>
        {
            bearer.RequireHttpsMetadata = false;
            bearer.SaveToken = true;
            bearer.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };

            bearer.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("Token has expired."));
                        return context.Response.WriteAsync(result);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("An unhandled error occurred."));
                        return context.Response.WriteAsync(result);
                    }
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resource."));
                        return context.Response.WriteAsync(result);
                    }

                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resource."));
                    return context.Response.WriteAsync(result);
                }
            };
        });

        services.AddAuthorization(options =>
        {
            foreach (var prop in typeof(SchoolPermissions).GetNestedTypes().SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null)?.ToString();
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue, policy => policy.RequireClaim(ClaimConstants.Permission, propertyValue));
                }
            }
        });

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        return app
            .UseMultiTenant();
    }
}
