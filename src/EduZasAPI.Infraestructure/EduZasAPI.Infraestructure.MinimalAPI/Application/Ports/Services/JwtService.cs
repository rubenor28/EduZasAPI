using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

public class JwtService
{
    private JwtSettings _jwtSettings;

    public JwtService(JwtSettings settings)
    {
        _jwtSettings = settings;
    }

    public string Generate(AuthPayload payload)
    {
        var claims = new List<Claim>
        {
          new Claim(JwtRegisteredClaimNames.Sub, payload.Id.ToString()),
          new Claim(ClaimTypes.Role, payload.Role.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresMinutes),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }
}
