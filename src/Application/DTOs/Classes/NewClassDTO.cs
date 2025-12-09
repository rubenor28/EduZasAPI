namespace Application.DTOs.Classes;

/// <summary>
/// Representa un profesor asociado a la creación de una clase.
/// </summary>
public sealed record Professor
{
    /// <summary>
    /// Identificador del usuario profesor.
    /// </summary>
    public required ulong UserId { get; init; }

    /// <summary>
    /// Indica si el profesor es el propietario principal.
    /// </summary>
    public required bool IsOwner { get; init; }
};

/// <summary>
/// Representa los datos requeridos para crear una nueva clase en el sistema.
/// </summary>
public sealed record NewClassDTO
{
    /// <summary>
    /// Identificador opcional para la clase (si se conoce de antemano).
    /// </summary>
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
    /// Código de color hexadecimal para la representación visual de la clase.
    /// </summary>
    public required string Color { get; init; }

    /// <summary>
    /// Identificador del profesor propietario de la clase.
    /// </summary>
    public required ulong OwnerId { get; init; }

    /// <summary>
    /// Lista de profesores adicionales a asociar durante la creación.
    /// </summary>
    public ICollection<Professor> Professors { get; init; } = [];
}
