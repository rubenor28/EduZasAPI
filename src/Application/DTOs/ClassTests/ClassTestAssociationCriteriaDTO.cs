using Domain.ValueObjects;

namespace Application.DTOs.ClassTests;

/// <summary>
/// Criterios para buscar asociaciones de evaluaciones en clases.
/// </summary>
public sealed record ClassTestAssociationCriteriaDTO : CriteriaDTO
{
    /// <summary>ID del profesor propietario.</summary>
    public required ulong ProfessorId { get; init; }

    /// <summary>ID de la evaluación.</summary>
    public required Guid TestId { get; init; }
}
