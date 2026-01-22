using Domain.ValueObjects;

namespace Application.DTOs.ClassResources;

/// <summary>
/// Criterios para buscar asociaciones de recursos en clases.
/// </summary>
public sealed record ClassResourceAssociationCriteriaDTO : CriteriaDTO
{
    /// <summary>ID del profesor propietario.</summary>
    public required ulong ProfessorId { get; init; }

    /// <summary>ID del recurso.</summary>
    public required Guid ResourceId { get; init; }
}
