namespace Application.DTOs.ClassProfessors;

/// <summary>
/// Datos para asignar un profesor a una clase.
/// </summary>
public sealed record NewClassProfessorDTO
{
    /// <summary>ID del usuario profesor.</summary>
    public required ulong UserId { get; init; }

    /// <summary>ID de la clase.</summary>
    public required string ClassId { get; init; }

    /// <summary>Indica si será propietario.</summary>
    public required bool IsOwner { get; init; }
}
