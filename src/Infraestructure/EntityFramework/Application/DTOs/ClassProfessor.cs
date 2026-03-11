namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de relación Clase-Profesor.
/// </summary>
public partial class ClassProfessor
{
    /// <summary>
    /// ID de la clase.
    /// </summary>
    public string ClassId { get; set; } = null!;

    /// <summary>
    /// ID del profesor.
    /// </summary>
    public ulong ProfessorId { get; set; }

    /// <summary>
    /// Indica si el profesor es el propietario de la clase.
    /// </summary>
    public bool? IsOwner {get; set; }

    /// <summary>
    /// Fecha de asignación del profesor a la clase.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Clase a la que pertenece el profesor.
    /// </summary>
    public virtual Class Class { get; set; } = null!;

    /// <summary>
    /// Profesor asociado a la clase.
    /// </summary>
    public virtual User Professor { get; set; } = null!;
}
