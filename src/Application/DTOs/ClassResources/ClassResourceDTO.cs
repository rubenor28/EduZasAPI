namespace Application.DTOs.ClassResources;

/// <summary>
/// Datos para asociar un recurso a una clase.
/// </summary>
public sealed class ClassResourceDTO
{
    /// <summary>ID de la clase.</summary>
    public required string ClassId { get; init; }

    /// <summary>ID del recurso.</summary>
    public required Guid ResourceId { get; init; }

    /// <summary>Indica si el recurso debe estar oculto.</summary>
    public bool Hidden { get; init; }
}
