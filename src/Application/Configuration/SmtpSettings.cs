/// <summary>
/// Contiene la configuración requerida para conectarse a un servidor SMTP.
/// </summary>
/// <remarks>
/// Esta clase se utiliza para vincular los ajustes de configuración desde `appsettings.json` o variables de entorno,
/// y se inyecta mediante `IOptions<SmtpSettings>` en la implementación del servicio de correo.
/// </remarks>
namespace Application.Configuration;

public class SmtpSettings
{
    /// <summary>
    /// Obtiene o establece el nombre del host o la dirección IP del servidor SMTP.
    /// </summary>
    public required string Server { get; set; }

    /// <summary>
    /// Obtiene o establece el puerto del servidor SMTP.
    /// </summary>
    public required int Port { get; set; }

    /// <summary>
    /// Obtiene o establece el nombre que se mostrará como remitente del correo.
    /// </summary>
    public required string SenderName { get; set; }

    /// <summary>
    /// Obtiene o establece la dirección de correo electrónico del remitente.
    /// </summary>
    public required string SenderEmail { get; set; }

    /// <summary>
    /// Obtiene o establece el nombre de usuario para la autenticación en el servidor SMTP.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Obtiene o establece la contraseña para la autenticación en el servidor SMTP.
    /// </summary>
    public required string Password { get; set; }
}
