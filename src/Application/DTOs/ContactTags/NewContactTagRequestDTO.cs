namespace Application.DTOs.ContactTags;

/// <summary>
/// DTO para la solicitud de creación de una etiqueta de contacto desde la UI.
/// </summary>
public sealed record NewContactTagRequestDTO
{
    /// <summary>ID del dueño de la agenda.</summary>
    public required ulong AgendaOwnerId { get; init; }

    /// <summary>ID del contacto.</summary>
    public required ulong UserId { get; init; }

    /// <summary>Texto de la etiqueta a crear/asignar.</summary>
    public required string TagText { get; init; }
}
