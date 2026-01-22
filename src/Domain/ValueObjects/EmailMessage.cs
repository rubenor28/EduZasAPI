namespace Domain.ValueObjects;

/// <summary>
/// Representa un mensaje de correo electrónico.
/// </summary>
public class EmailMessage
{
    /// <summary>Destinatarios principales.</summary>
    public required IReadOnlyList<string> To { get; init; }

    /// <summary>Destinatarios en copia (CC).</summary>
    public IReadOnlyList<string>? Cc { get; init; }

    /// <summary>Destinatarios en copia oculta (BCC).</summary>
    public IReadOnlyList<string>? Bcc { get; init; }

    /// <summary>Asunto del correo.</summary>
    public required string Subject { get; init; }

    /// <summary>Cuerpo del mensaje.</summary>
    public required string Body { get; init; }

    /// <summary>Indica si el cuerpo es HTML. Por defecto es true.</summary>
    public bool IsBodyHtml { get; init; } = true;
}
