namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

public class JwtSettings
{
    public required string Secret { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required TimeSpan ExpiresMinutes { get; init; }
}
