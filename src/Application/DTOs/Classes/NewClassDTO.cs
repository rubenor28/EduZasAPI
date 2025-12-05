namespace Application.DTOs.Classes;

public sealed record Professor
{
    public required ulong UserId { get; init; }
    public required bool IsOwner { get; init; }
};

/// <summary>
/// Representa los datos requeridos para crear una nueva clase en el sistema.
/// </summary>
public sealed record NewClassDTO
{
    public string Id { get; init; } = string.Empty;

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

    /// <summary>
    /// Id del profesor dueño de la clase nueva
    /// </summary>
    public required ulong OwnerId { get; init; }

    /// <summary>
    /// Informacion para agregar profesores durante la creacion
    /// </summary>
    public ICollection<Professor> Professors { get; init; } = [];
}
