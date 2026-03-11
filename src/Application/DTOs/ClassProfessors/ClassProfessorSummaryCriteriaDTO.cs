using Domain.ValueObjects;

namespace Application.DTOs.ClassProfessors;

/// <summary>
/// DTO que representa los criterios de búsqueda para obtener un resumen de profesores de una clase.
/// </summary>
public record ClassProfessorSummaryCriteriaDTO() : CriteriaDTO
{
    /// <summary>
    /// Obtiene o inicializa el ID de la clase para la cual se buscan los profesores.
    /// </summary>
    public required string ClassId { get; init; }
    /// <summary>
    /// Obtiene o inicializa el ID del profesor para el cual se busca el resumen.
    /// </summary>
    public required ulong ProfessorId { get; init; }
};
