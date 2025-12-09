namespace Application.DTOs.ClassStudents;

/// <summary>
/// Datos para actualizar una relación estudiante-clase.
/// </summary>
public sealed record ClassStudentUpdateDTO
{
    /// <summary>ID del estudiante.</summary>
    public required ulong UserId { get; init; }

    /// <summary>ID de la clase.</summary>
    public required string ClassId { get; init; }

    /// <summary>Indica si el estudiante está oculto en la clase.</summary>
    public required bool Hidden { get; init; }
}
