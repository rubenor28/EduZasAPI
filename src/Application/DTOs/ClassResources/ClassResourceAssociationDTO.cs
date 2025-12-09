namespace Application.DTOs.ClassResources;

/// <summary>
/// Detalles de la asociación de un recurso con una clase.
/// </summary>
public sealed record ClassResourceAssociationDTO
{
    /// <summary>ID de la clase.</summary>
    public required string ClassId { get; init; }

    /// <summary>Nombre de la clase.</summary>
    public required string ClassName { get; init; }

    /// <summary>Indica si el recurso está asociado a esta clase.</summary>
    public required bool IsAssociated { get; init; }

    /// <summary>Indica si el recurso está oculto en esta clase.</summary>
    public required bool IsHidden { get; init; }
}