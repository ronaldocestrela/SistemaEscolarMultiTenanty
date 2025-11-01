using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Indentity.Models;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
}
