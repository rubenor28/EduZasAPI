namespace Application.DTOs.ContactTags;

/// <summary>
/// Datos para asignar una etiqueta a un contacto.
/// </summary>
public sealed record NewContactTagDTO
{
    /// <summary>Texto de la etiqueta.</summary>
    public required string Tag { get; init; }

    /// <summary>ID del dueño de la agenda.</summary>
    public required ulong AgendaOwnerId { get; init; }

    /// <summary>ID del contacto.</summary>
    public required ulong UserId { get; init; }
}
