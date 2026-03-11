namespace Application.Configuration;

/// <summary>
/// Configuración para la conexión a un servidor SMTP (Simple Mail Transfer Protocol).
/// </summary>
public class SmtpSettings
{
    /// <summary>
    /// Obtiene la dirección del servidor SMTP (host o IP).
    /// </summary>
    /// <value>
    /// Una cadena de texto que representa el host o la dirección IP del servidor SMTP.
    /// </value>
    public required string Server { get; init; }

    /// <summary>
    /// Obtiene el puerto utilizado para la conexión al servidor SMTP.
    /// </summary>
    /// <value>
    /// Un valor entero que representa el número de puerto del servidor SMTP.
    /// </value>
    public required int Port { get; init; }

    /// <summary>
    /// Obtiene el nombre del remitente que se mostrará en los correos electrónicos.
    /// </summary>
    /// <value>
    /// Una cadena de texto que contiene el nombre del remitente.
    /// </value>
    public required string SenderName { get; init; }

    /// <summary>
    /// Obtiene la dirección de correo electrónico del remitente.
    /// </summary>
    /// <value>
    /// Una cadena de texto que contiene la dirección de correo electrónico del remitente.
    /// </value>
    public required string SenderEmail { get; init; }

    /// <summary>
    /// Obtiene el nombre de usuario para la autenticación en el servidor SMTP.
    /// </summary>
    /// <value>
    /// Una cadena de texto que contiene el nombre de usuario para la autenticación.
    /// </value>
    public required string Username { get; init; }

    /// <summary>
    /// Obtiene la contraseña para la autenticación en el servidor SMTP.
    /// </summary>
    /// <value>
    /// Una cadena de texto que contiene la contraseña para la autenticación.
    /// </value>
    public required string Password { get; init; }
}
