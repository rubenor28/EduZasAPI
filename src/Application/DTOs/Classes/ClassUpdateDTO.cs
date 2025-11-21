using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Classes;

/// <summary>
/// Representa los datos para actualizar la información de una clase existente en el sistema.
/// </summary>
public sealed record ClassUpdateDTO
{
    /// <summary>
    /// Obtiene o establece el identificador único de la clase a actualizar.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Obtiene o establece el estado de la clase.
    /// </summary>
    public required bool Active { get; set; }

    /// <summary>
    /// Obtiene o establece el nombre de la clase.
    /// </summary>
    public required string ClassName { get; set; }

    /// <summary>
    /// Color de la carta en la UI
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// Obtiene o establece la materia o asignatura de la clase (opcional).
    /// </summary>
    public required Optional<string> Subject { get; set; }

    /// <summary>
    /// Obtiene o establece la sección o grupo de la clase (opcional).
    /// </summary>
    public required Optional<string> Section { get; set; }

    /// <summary>
    /// Representa el ejecutor de la operacion
    /// </summary>
    public required Executor Executor { get; set; }
}
