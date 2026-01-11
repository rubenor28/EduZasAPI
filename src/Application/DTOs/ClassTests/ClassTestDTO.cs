namespace Application.DTOs.ClassTests;

/// <summary>
/// Datos para asociar una evaluación a una clase.
/// </summary>
public sealed record ClassTestDTO
{
    /// <summary>ID de la evaluación.</summary>
    public required Guid TestId { get; init; }

    /// <summary>ID de la clase.</summary>
    public required string ClassId { get; init; }
}
