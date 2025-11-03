using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Indentity.Auth;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; set; } = permission;
}
