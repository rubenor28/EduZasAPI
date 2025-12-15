using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MinimalAPI.Application.DTOs;

namespace MinimalAPI.Application.Services;

/// <summary>
/// Servicio para generar tokens JWT basados en la información de un usuario autenticado.
/// </summary>
public class JwtService(JwtSettings jwtSettings)
{
    /// <summary>
    /// Genera un token JWT firmado que representa la identidad del usuario.
    /// </summary>
    /// <param name="payload">Información del usuario que se incluirá en el token.</param>
    /// <returns>Token JWT como cadena de texto.</returns>
    public string Generate(AuthPayload payload)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, payload.Id.ToString()),
            new(ClaimTypes.Role, payload.Role.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiresMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }
}
