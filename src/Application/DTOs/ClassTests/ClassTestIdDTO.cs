namespace Application.DTOs.ClassTests;

/// <summary>
/// Identificador compuesto para una relación clase-evaluación.
/// </summary>
public sealed record ClassTestIdDTO
{
    /// <summary>ID de la evaluación.</summary>
    public required Guid TestId { get; init; }

    /// <summary>ID de la clase.</summary>
    public required string ClassId { get; init; }
}
