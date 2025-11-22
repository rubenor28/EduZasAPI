using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Classes;

public sealed record Professor
{
    public required ulong UserId { get; set; }
    public required bool IsOwner { get; set; }
};

/// <summary>
/// Representa los datos requeridos para crear una nueva clase en el sistema.
/// </summary>
public sealed record NewClassDTO
{
    public string Id { get; set; } = string.Empty;

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
    /// Id del profesor dueño de la clase nueva
    /// </summary>
    public required ulong OwnerId { get; set; }

    /// <summary>
    /// Informacion para agregar profesores durante la creacion
    /// </summary>
    public required ICollection<Professor> Professors { get; set; }

    /// <summary>
    /// Ejecutor de la accion
    /// </summary>
    public required Executor Executor { get; set; }
}
