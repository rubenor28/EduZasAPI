namespace Domain.Entities;

/// <summary>
/// Representa la asignación de un recurso educativo a una clase específica.
/// </summary>
/// <remarks>
/// Esta entidad actúa como una tabla de unión entre <see cref="ClassDomain"/> y <see cref="ResourceDomain"/>.
/// Las propiedades <see cref="ClassId"/> y <see cref="ResourceId"/> juntas forman su clave primaria.
/// </remarks>
public sealed record ClassResourceDomain
{
    /// <summary>
    /// Obtiene o establece el identificador de la clase a la que se asigna el recurso.
    /// </summary>
    public required string ClassId { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador del recurso asignado.
    /// </summary>
    public required Guid ResourceId { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica si el recurso está oculto para los estudiantes de la clase.
    /// </summary>
    public required bool Hidden { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora en que se realizó la asignación.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}
