using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Representa una clase o curso académico.
/// </summary>
/// <remarks>
/// Esta clase implementa la interfaz <see cref="IIdentifiable{T}"/> para garantizar que cada
/// instancia tenga un identificador único.
/// </remarks>
public class ClassDomain : IIdentifiable<string>, ISoftDeletable
{
    /// <summary>
    /// Obtiene o establece el identificador único de la clase.
    /// </summary>
    /// <value>Identificador único de la clase. Se corresponde con <see cref="IIdentifiable{T}.Id"/>.</value>
    public required string Id { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica si la clase está activa.
    /// </summary>
    public required bool Active { get; set; }

    /// <summary>
    /// Obtiene o establece el nombre de la clase.
    /// </summary>
    public required string ClassName { get; set; }

    /// <summary>
    /// Color de la carta en la UI.
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// Obtiene o establece la materia a la que pertenece la clase.
    /// </summary>
    public required Optional<string> Subject { get; set; }

    /// <summary>
    /// Obtiene o establece la sección o grupo de la clase.
    /// </summary>
    public required Optional<string> Section { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación de la clase.
    /// </summary>
    public required DateTime CreatedAt { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la última modificación de la clase.
    /// </summary>
    public required DateTime ModifiedAt { get; set; }
}
