using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Auth;
using EduZasAPI.Application.Common;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EduZasAPI.Infraestructure.IdentityModel.Application.Common;

/// <summary>
/// Implementación del servicio de tokens JWT utilizando la biblioteca System.IdentityModel.Tokens.Jwt.
/// </summary>
/// <remarks>
/// Esta clase proporciona funcionalidades completas para la generación y validación de tokens JWT
/// según el estándar RFC 7519, incluyendo soporte para claims personalizados y validación de firma.
/// </remarks>
public class JwtService : ISignedTokenService
{
    private readonly TimeSpan _exp;
    private readonly SymmetricSecurityKey _secret;
    private readonly string _issuer;
    private readonly string _audience;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="JwtService"/>.
    /// </summary>
    /// <param name="settings">Configuración JWT con los parámetros necesarios.</param>
    public JwtService(JwtSettings settings)
    {
        _exp = TimeSpan.FromMinutes(settings.ExpirationMinutes);
        _secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
        _issuer = settings.Issuer;
        _audience = settings.Audience;
    }

    /// <summary>
    /// Genera un nuevo token JWT con el payload especificado.
    /// </summary>
    /// <typeparam name="T">Tipo del payload a incluir en el token. Debe ser no nulo.</typeparam>
    /// <param name="payload">Datos a incluir en el payload del token JWT.</param>
    /// <returns>Token JWT generado como cadena codificada.</returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando ocurre un error durante la generación del token.
    /// </exception>
    /// <remarks>
    /// El método convierte las propiedades del objeto payload en claims JWT
    /// y añade claims estándar como JTI (ID único) e IAT (timestamp de emisión).
    /// </remarks>
    public string Generate<T>(T payload) where T : notnull
    {
        try
        {
            var claims = ObjectToClaims(payload);

            claims.AddRange(GetStandardClaims());

            var credentials = new SigningCredentials(_secret, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _issuer,
                Audience = _audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_exp),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error generating JWT for type {typeof(T).Name}", ex);
        }
    }

    /// <summary>
    /// Valida la integridad, autenticidad y expiración de un token JWT.
    /// </summary>
    /// <typeparam name="T">Tipo esperado del payload del token JWT. Debe ser no nulo.</typeparam>
    /// <param name="secret">Secreto utilizado para verificar la firma del token.</param>
    /// <param name="token">Token JWT a validar.</param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> que contiene el payload decodificado si el token es válido,
    /// o un <see cref="SignedTokenErrors"/> que describe el error de validación.
    /// </returns>
    /// <remarks>
    /// Realiza validaciones completas incluyendo firma, emisor, audiencia y tiempo de expiración.
    /// </remarks>
    public Result<T, SignedTokenErrors> IsValid<T>(string secret, string token) where T : notnull
    {
        try
        {
            if (secret != Encoding.UTF8.GetString(_secret.Key))
                return Result<T, SignedTokenErrors>.Err(SignedTokenErrors.TokenInvalid);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _secret,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            var claims = principal.Claims;
            var payload = ClaimsToObject<T>(claims);

            return Result<T, SignedTokenErrors>.Ok(payload);
        }
        catch (SecurityTokenExpiredException)
        {
            return Result<T, SignedTokenErrors>.Err(SignedTokenErrors.TokenExpired);
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            return Result<T, SignedTokenErrors>.Err(SignedTokenErrors.TokenInvalid);
        }
        catch (SecurityTokenException)
        {
            return Result<T, SignedTokenErrors>.Err(SignedTokenErrors.TokenInvalid);
        }
        catch (Exception)
        {
            return Result<T, SignedTokenErrors>.Err(SignedTokenErrors.Unknown);
        }
    }

    /// <summary>
    /// Convierte las propiedades de un objeto en una lista de claims JWT.
    /// </summary>
    /// <typeparam name="T">Tipo del objeto a convertir.</typeparam>
    /// <param name="obj">Objeto cuyas propiedades se convertirán en claims.</param>
    /// <returns>Lista de claims generados a partir de las propiedades del objeto.</returns>
    private List<Claim> ObjectToClaims<T>(T obj)
    {
        var claims = new List<Claim>();
        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj)?.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                claims.Add(new Claim(prop.Name, value));
            }
        }

        return claims;
    }

    /// <summary>
    /// Reconstruye un objeto a partir de una colección de claims JWT.
    /// </summary>
    /// <typeparam name="T">Tipo del objeto a reconstruir.</typeparam>
    /// <param name="claims">Colección de claims JWT.</param>
    /// <returns>Instancia del objeto reconstruido a partir de los claims.</returns>
    private T ClaimsToObject<T>(IEnumerable<Claim> claims) where T : notnull
    {
        var instance = Activator.CreateInstance<T>();
        var properties = typeof(T).GetProperties();
        var claimsDict = claims.ToDictionary(c => c.Type, c => c.Value);

        foreach (var prop in properties)
        {
            if (claimsDict.TryGetValue(prop.Name, out var value) && !string.IsNullOrEmpty(value))
            {
                var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(instance, convertedValue);
            }
        }

        return instance;
    }

    /// <summary>
    /// Obtiene los claims estándar JWT que se incluyen en todos los tokens generados.
    /// </summary>
    /// <returns>Lista de claims estándar JWT.</returns>
    /// <remarks>
    /// Incluye JTI (ID único del token) e IAT (timestamp de emisión).
    /// </remarks>
    private List<Claim> GetStandardClaims()
    {
        return new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };
    }
}
