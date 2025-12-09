namespace Application.DTOs.ClassTests;

/// <summary>
/// Detalles de la asociación de una evaluación con una clase.
/// </summary>
public sealed record ClassTestAssociationDTO
{
    /// <summary>ID de la clase.</summary>
    public required string ClassId { get; init; }

    /// <summary>Nombre de la clase.</summary>
    public required string ClassName { get; init; }

    /// <summary>Indica si la evaluación está asociada a esta clase.</summary>
    public required bool IsAssociated { get; init; }

    /// <summary>Indica si la evaluación es visible para los estudiantes.</summary>
    public required bool IsVisible { get; init; }
}
