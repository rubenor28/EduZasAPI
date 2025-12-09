using Application.DTOs.Common;

namespace Application.DTOs.Tags;

/// <summary>
/// Criterios de búsqueda para etiquetas.
/// </summary>
public sealed record TagCriteriaDTO : CriteriaDTO
{
    /// <summary>Filtra por texto.</summary>
    public StringQueryDTO? Text { get; init; } 

    /// <summary>Filtra por ID del dueño de la agenda.</summary>
    public ulong? AgendaOwnerId { get; init; } 

    /// <summary>Filtra por ID de contacto asociado.</summary>
    public ulong? ContactId { get; init; } 
}
