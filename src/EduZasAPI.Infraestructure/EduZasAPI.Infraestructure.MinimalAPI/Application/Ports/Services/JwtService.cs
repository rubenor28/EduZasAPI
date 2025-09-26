using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

/// <summary>
/// Servicio para generar tokens JWT basados en la información de un usuario autenticado.
/// </summary>
public class JwtService
{
    private JwtSettings _jwtSettings;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="JwtService"/> con la configuración de JWT proporcionada.
    /// </summary>
    /// <param name="settings">Configuración de JWT que incluye clave secreta y tiempo de expiración.</param>
    public JwtService(JwtSettings settings)
    {
        _jwtSettings = settings;
    }

    /// <summary>
    /// Genera un token JWT firmado que representa la identidad del usuario.
    /// </summary>
    /// <param name="payload">Información del usuario que se incluirá en el token.</param>
    /// <returns>Token JWT como cadena de texto.</returns>
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
