
namespace Application.DTOs.Classes;

/// <summary>
/// Representa los datos para actualizar la información de una clase existente en el sistema.
/// </summary>
public sealed record ClassUpdateDTO
{
    /// <summary>
    /// Obtiene o establece el identificador único de la clase a actualizar.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Obtiene o establece el estado de la clase.
    /// </summary>
    public required bool Active { get; init; }

    /// <summary>
    /// Obtiene o establece el nombre de la clase.
    /// </summary>
    public required string ClassName { get; init; }

    /// <summary>
    /// Obtiene o establece la materia o asignatura de la clase (opcional).
    /// </summary>
    public string? Subject { get; init; }

    /// <summary>
    /// Obtiene o establece la sección o grupo de la clase (opcional).
    /// </summary>
    public string? Section { get; init; }

    /// <summary>
    /// Color de la carta en la UI
    /// </summary>
    public required string Color { get; init; }
}
