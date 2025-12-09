namespace Application.DTOs.ClassResources;

/// <summary>
/// Identificador compuesto para una relación clase-recurso.
/// </summary>
public sealed record ClassResourceIdDTO
{
    /// <summary>ID de la clase.</summary>
    public required string ClassId { get; init; }

    /// <summary>ID del recurso.</summary>
    public required Guid ResourceId { get; init; }
}
