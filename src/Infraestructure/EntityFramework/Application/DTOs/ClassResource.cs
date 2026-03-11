namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de relación Clase-Recurso.
/// </summary>
public partial class ClassResource
{
    /// <summary>
    /// ID de la clase.
    /// </summary>
    public string ClassId { get; set; } = null!;

    /// <summary>
    /// ID del recurso.
    /// </summary>
    public Guid ResourceId { get; set; }

    /// <summary>
    /// Indica si el recurso está oculto en la clase.
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// Fecha de asignación del recurso a la clase.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Clase a la que pertenece el recurso.
    /// </summary>
    public virtual Class Class { get; set; } = null!;

    /// <summary>
    /// Recurso asociado a la clase.
    /// </summary>
    public virtual Resource Resource { get; set; } = null!;
}
