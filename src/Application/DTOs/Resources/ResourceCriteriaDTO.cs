using Domain.ValueObjects;

namespace Application.DTOs.Resources;

/// <summary>
/// Criterios de búsqueda y filtrado para recursos académicos.
/// </summary>
public sealed record ResourceCriteriaDTO : CriteriaDTO
{
    /// <summary>
    /// Filtrar por estado de activación.
    /// </summary>
    public bool? Active { get; init; } 

    /// <summary>
    /// Filtrar por título del recurso.
    /// </summary>
    public StringQueryDTO? Title { get; init; } 

    /// <summary>
    /// Filtrar por ID del profesor creador.
    /// </summary>
    public ulong? ProfessorId { get; init; } 

    /// <summary>
    /// Filtrar por recursos asociados a una clase específica.
    /// </summary>
    public string? ClassId { get; init; } 
}
