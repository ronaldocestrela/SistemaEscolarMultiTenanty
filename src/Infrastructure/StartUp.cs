using Finbuckle.MultiTenant;
using Infrastructure.Contexts;
using Infrastructure.Indentity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            .AddIdentityServices();
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
            .Services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        return app
            .UseMultiTenant();
    }
}
