/// <summary>
/// Configuración para conexión SMTP.
/// </summary>
public class SmtpSettings
{
    /// <summary>Host o IP del servidor.</summary>
    public required string Server { get; set; }

    /// <summary>Puerto del servidor.</summary>
    public required int Port { get; set; }

    /// <summary>Nombre del remitente.</summary>
    public required string SenderName { get; set; }

    /// <summary>Email del remitente.</summary>
    public required string SenderEmail { get; set; }

    /// <summary>Usuario de autenticación.</summary>
    public required string Username { get; set; }

    /// <summary>Contraseña de autenticación.</summary>
    public required string Password { get; set; }
}
