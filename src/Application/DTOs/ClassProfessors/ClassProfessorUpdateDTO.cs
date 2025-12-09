namespace Application.DTOs.ClassProfessors;

/// <summary>
/// Datos para actualizar una relación profesor-clase.
/// </summary>
public sealed record ClassProfessorUpdateDTO
{
    /// <summary>ID del usuario profesor.</summary>
    public required ulong UserId { get; init; }

    /// <summary>ID de la clase.</summary>
    public required string ClassId { get; init; }

    /// <summary>Indica si es propietario de la clase.</summary>
    public required bool IsOwner { get; init; }
}
