namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

public class JwtSettings
{
    public required string Secret { get; init; }
    public required int ExpiresMinutes { get; init; }
}
