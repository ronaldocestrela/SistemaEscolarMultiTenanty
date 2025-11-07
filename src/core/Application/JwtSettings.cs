namespace Application;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public int TokenExpiryTimeMinutes { get; set; }
    public int RefreshTokenExpiryTimeDays { get; set; }
}
