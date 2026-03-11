namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de relación Clase-Estudiante.
/// </summary>
public partial class ClassStudent
{
    /// <summary>
    /// ID de la clase.
    /// </summary>
    public string ClassId { get; set; } = null!;

    /// <summary>
    /// ID del estudiante.
    /// </summary>
    public ulong StudentId { get; set; }

    /// <summary>
    /// Indica si el estudiante está oculto en la clase.
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// Fecha de inscripción del estudiante en la clase.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Clase a la que pertenece el estudiante.
    /// </summary>
    public virtual Class Class { get; set; } = null!;

    /// <summary>
    /// Estudiante asociado a la clase.
    /// </summary>
    public virtual User Student { get; set; } = null!;
}
