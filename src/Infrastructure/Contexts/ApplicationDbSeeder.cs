using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Indentity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class ApplicationDbSeeder(
    IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor,
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext applicationDbContext
    )
{
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantInfoContextAccessor = tenantInfoContextAccessor;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        if (_applicationDbContext.Database.GetMigrations().Any())
        {
            if ((await _applicationDbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
            {
                await _applicationDbContext.Database.MigrateAsync(cancellationToken);
            }

            // Seeding 
            if (await _applicationDbContext.Database.CanConnectAsync(cancellationToken))
            {
                // Default Roles > Assign permissions/claims
                await InitializeDefaultRolesAsync(cancellationToken);
                
                // User > Assign Roles
                await InitializeAdminUserAsync(cancellationToken);
            }
        }
    }

    private async Task InitializeDefaultRolesAsync(CancellationToken cancellationToken)
    {
        foreach (var roleName in RoleConstants.DefaultRoles)
        {
            if (await _roleManager.Roles.SingleOrDefaultAsync(role => role.Name == roleName, cancellationToken) is not ApplicationRole incomingRole)
            {
                incomingRole = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} role",
                    NormalizedName = roleName.ToUpperInvariant()
                };

                await _roleManager.CreateAsync(incomingRole);
            }

            // Assign Role Claims/Permissions
            if (roleName == RoleConstants.Basic)
            {
                // Assign Basic Role Claims/Permissions
                await AssignPermissionsToRoleAsync(SchoolPermissions.Basic, incomingRole, cancellationToken);
            }
            else if (roleName == RoleConstants.Admin)
            {
                // Assign Admin Role Claims/Permissions
                await AssignPermissionsToRoleAsync(SchoolPermissions.Admin, incomingRole, cancellationToken);

                if (_tenantInfoContextAccessor.MultiTenantContext.TenantInfo?.Id == TenancyConstants.Root.Id)
                {
                    await AssignPermissionsToRoleAsync(SchoolPermissions.Root, incomingRole, cancellationToken);
                }
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(IReadOnlyList<SchoolPermission> rolePermissions, ApplicationRole role, CancellationToken cancellationToken)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var rolePermission in rolePermissions)
        {
            if (!currentClaims.Any(c => c.Type == ClaimConstants.Permission && c.Value == rolePermission.Name))
            {
                await _applicationDbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = rolePermission.Name,
                    Description = rolePermission.Description,
                    Group = rolePermission.Group
                }, cancellationToken);

                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task InitializeAdminUserAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_tenantInfoContextAccessor.MultiTenantContext.TenantInfo?.Email)) return;

        if (await _userManager.Users.SingleOrDefaultAsync(user => user.Email == _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email, cancellationToken) is not ApplicationUser incomingUser)
        {
            incomingUser = new ApplicationUser
            {
                UserName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email,
                Email = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email,
                FirstName = TenancyConstants.FirstName,
                LastName = TenancyConstants.LastName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };

            await _userManager.CreateAsync(incomingUser, TenancyConstants.DefaultPassword);
        }

        if (!await _userManager.IsInRoleAsync(incomingUser, RoleConstants.Admin))
        {
            await _userManager.AddToRoleAsync(incomingUser, RoleConstants.Admin);
        }
    }
}
