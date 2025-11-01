using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tenancy;

public class TenantDbSeeder(TenantDbContext tenantDbContext, IServiceProvider serviceProvider) : ITenantDbSeeder
{
    private readonly TenantDbContext _tenantDbContext = tenantDbContext;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        // Seed Tenant data
        await InitializeDatabaseWithTenantAsync(cancellationToken);

        foreach (var tenant in await _tenantDbContext.TenantInfo.ToListAsync(cancellationToken))
        {
            // Application DBseeder
            await InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
        }
    }

    private async Task InitializeDatabaseWithTenantAsync(CancellationToken cancellationToken)
    {
        // Seed data within tenant's database context
        if (await _tenantDbContext.TenantInfo.FindAsync([TenancyConstants.Root.Id], cancellationToken) is null)
        {
            // Create tenant
            var rootTenant = new ABCSchoolTenantInfo
            {
                Id = TenancyConstants.Root.Id,
                Identifier = TenancyConstants.Root.Id,
                Name = TenancyConstants.Root.Name,
                Email = TenancyConstants.Root.Email,
                FirstName = TenancyConstants.FirstName,
                LastName = TenancyConstants.LastName,
                IsActive = true,
                ValidUpTo = DateTime.UtcNow.AddYears(2)
            };

            await _tenantDbContext.TenantInfo.AddAsync(rootTenant, cancellationToken);
            await _tenantDbContext.SaveChangesAsync(cancellationToken);
        }
    }
    
    private async Task InitializeApplicationDbForTenantAsync(ABCSchoolTenantInfo currentTenant, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        _serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
            .MultiTenantContext = new MultiTenantContext<ABCSchoolTenantInfo>()
            {
                TenantInfo = currentTenant
            };
        
        await scope.ServiceProvider
            .GetRequiredService<ApplicationDbSeeder>()
            .InitializeDatabaseAsync(cancellationToken);
    }
}
