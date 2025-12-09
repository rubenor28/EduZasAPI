namespace Application.DTOs.Contacts;

/// <summary>
/// Datos para crear un nuevo contacto.
/// </summary>
public sealed record NewContactDTO
{
    /// <summary>Alias del contacto.</summary>
    public required string Alias { get; init; }

    /// <summary>Notas adicionales.</summary>
    public string? Notes { get; init; }

    /// <summary>ID del dueño de la agenda.</summary>
    public required ulong AgendaOwnerId { get; init; }

    /// <summary>ID del usuario a agregar.</summary>
    public required ulong UserId { get; init; }

    /// <summary>Etiquetas iniciales.</summary>
    public IEnumerable<string>? Tags { get; init; }
}
