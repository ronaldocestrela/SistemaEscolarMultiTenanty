using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application;
using Application.Exceptions;
using Application.Features.Identity.Tokens;
using Azure.Core;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Indentity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Indentity.Tokens;

public class TokenService(UserManager<ApplicationUser> userManager,
    IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor,
    RoleManager<ApplicationRole> roleManager,
    IOptions<JwtSettings> jwtSettings) : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor = tenantContextAccessor;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<TokenResponse> LoginAsync(TokenRequest request)
    {
        #region  Validation
        if (request == null) throw new UnauthorizedException(["Invalid login request."]);

        if (_tenantContextAccessor.MultiTenantContext?.TenantInfo?.IsActive != true) throw new UnauthorizedException(["Tenant subscription is not active. Contact support."]);

        if (string.IsNullOrWhiteSpace(request.Username)) throw new UnauthorizedException(["Username must be provided."]);
        if (string.IsNullOrWhiteSpace(request.Password)) throw new UnauthorizedException(["Password must be provided."]);

        var userInDb = await _userManager.FindByNameAsync(request.Username)
            ?? throw new UnauthorizedException(["Authentication failed."]);

        if (!await _userManager.CheckPasswordAsync(userInDb, request.Password)) throw new UnauthorizedException(["Incorrect Username or password."]);

        if (!userInDb.IsActive) throw new UnauthorizedException(["User is not active. Contact administrator."]);

        if (_tenantContextAccessor.MultiTenantContext?.TenantInfo.Id is not TenancyConstants.Root.Id)
        {
            if (_tenantContextAccessor.MultiTenantContext?.TenantInfo.ValidUpTo < DateTime.UtcNow)
            {
                throw new UnauthorizedException(["Subscription has expired. Contact support."]);
            }
        }
        #endregion

        // Generate JWT
        return await GenerateTokenAndUpdateUserAsync(userInDb);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        if (request.CurrentJwt == null) throw new UnauthorizedException(["Invalid refresh token request."]);
        var userPrincipal = GetClaimsPrincipalFromExpiredToken(request.CurrentJwt);
        var userEmail = userPrincipal.GetEmail();

        var userInDb = await _userManager.FindByEmailAsync(userEmail)
            ?? throw new UnauthorizedException(["User not found."]);

        if (userInDb.RefreshToken != request.CurrentRefreshToken || userInDb.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedException(["Invalid refresh token request."]);
        }
        
        return await GenerateTokenAndUpdateUserAsync(userInDb);
    }

    private ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string expiringToken)
    {
        var tkValidationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(expiringToken, tkValidationParams, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedException(["Invalid token provided. Failed to generate new token."]);
        }

        return principal;
    }

    private async Task<TokenResponse> GenerateTokenAndUpdateUserAsync(ApplicationUser user)
    {
        // Implementation for generating JWT token
        var newJwt = await GenerateToken(user);

        // Refresh Token
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryTimeDays);

        await _userManager.UpdateAsync(user);

        return new TokenResponse
        {
            Jwt = newJwt,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryDate = user.RefreshTokenExpiryTime
        };
    }

    private async Task<string> GenerateToken(ApplicationUser user)
    {
        // Generate JWT token
        return GenerateEncryptedToken(GenerateSigningCredentials(), await GetUserClaims(user));
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        // Generate encrypted token
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.TokenExpiryTimeMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private SigningCredentials GenerateSigningCredentials()
    {
        // Generate signing credentials
        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    private async Task<IEnumerable<Claim>> GetUserClaims(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();

        foreach(var userRole in userRoles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, userRole));
            var currentRole = await _roleManager.FindByNameAsync(userRole);
            if (currentRole != null)
            {
                var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
                permissionClaims.AddRange(allPermissionsForCurrentRole.Where(c => c.Type == ClaimConstants.Permission));
            }

        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FirstName!),
            new(ClaimTypes.Surname, user.LastName!),
            new(ClaimConstants.Tenant, _tenantContextAccessor.MultiTenantContext?.TenantInfo?.Id ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        }
        .Union(roleClaims)
        .Union(userClaims)
        .Union(permissionClaims);

        return claims;
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
