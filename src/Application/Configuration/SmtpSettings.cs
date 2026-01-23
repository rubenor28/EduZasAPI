namespace Application.Configuration;

/// <summary>
/// Configuración para conexión SMTP.
/// </summary>
public class SmtpSettings
{
    /// <summary>Host o IP del servidor.</summary>
    public required string Server { get; init; }

    /// <summary>Puerto del servidor.</summary>
    public required int Port { get; init; }

    /// <summary>Nombre del remitente.</summary>
    public required string SenderName { get; init; }

    /// <summary>Email del remitente.</summary>
    public required string SenderEmail { get; init; }

    /// <summary>Usuario de autenticación.</summary>
    public required string Username { get; init; }

    /// <summary>Contraseña de autenticación.</summary>
    public required string Password { get; init; }
}
