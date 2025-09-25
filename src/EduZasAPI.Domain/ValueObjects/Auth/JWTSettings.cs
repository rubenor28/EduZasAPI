namespace EduZasAPI.Domain.Auth;

/// <summary>
/// Representa la configuración necesaria para la generación y validación de tokens JWT.
/// </summary>
/// <remarks>
/// Esta clase se utiliza para almacenar los parámetros de configuración requeridos
/// por los servicios de autenticación basados en JWT (JSON Web Tokens).
/// </remarks>
public class JwtSettings
{
    /// <summary>
    /// Obtiene el secreto utilizado para firmar y verificar los tokens JWT.
    /// </summary>
    /// <value>Cadena secreta para la firma criptográfica. Campo obligatorio.</value>
    /// <remarks>
    /// Este secreto debe ser una cadena segura y mantenerse confidencial,
    /// ya que es utilizado para garantizar la integridad y autenticidad de los tokens.
    /// </remarks>
    public required string Secret { get; init; }

    /// <summary>
    /// Obtiene el tiempo de expiración de los tokens JWT en minutos.
    /// </summary>
    /// <value>Duración de validez del token en minutos. Campo obligatorio.</value>
    /// <remarks>
    /// Este valor determina por cuánto tiempo será válido un token JWT después de su emisión.
    /// </remarks>
    public required int ExpirationMinutes { get; init; }

    /// <summary>
    /// Obtiene el emisor (issuer) de los tokens JWT.
    /// </summary>
    /// <value>Identificador del emisor del token. Campo obligatorio.</value>
    /// <remarks>
    /// Este campo representa la entidad que crea y firma el token, típicamente
    /// la aplicación o servicio que genera el token de autenticación.
    /// </remarks>
    public required string Issuer { get; init; }

    /// <summary>
    /// Obtiene la audiencia (audience) de los tokens JWT.
    /// </summary>
    /// <value>Identificador del destinatario del token. Campo obligatorio.</value>
    /// <remarks>
    /// Este campo especifica para quién está destinado el token, típicamente
    /// la aplicación o servicio que debe aceptar y validar el token.
    /// </remarks>
    public required string Audience { get; init; }
}
