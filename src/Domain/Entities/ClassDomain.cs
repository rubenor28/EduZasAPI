namespace Domain.Entities;

/// <summary>
/// Representa una clase o curso académico en el sistema.
/// </summary>
/// <remarks>
/// Cada clase es un contenedor principal para estudiantes, profesores, recursos y exámenes.
/// Su identificador único, <see cref="Id"/>, es utilizado para vincularla con estas otras entidades.
/// </remarks>
public class ClassDomain
{
    /// <summary>
    /// Obtiene o establece el identificador único de la clase.
    /// </summary>
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
    /// Obtiene o establece el color asociado a la clase, usado para la UI.
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// Obtiene o establece la materia a la que pertenece la clase (opcional).
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Obtiene o establece la sección o grupo de la clase (opcional).
    /// </summary>
    public string? Section { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación de la clase.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la última modificación de la clase.
    /// </summary>
    public required DateTimeOffset ModifiedAt { get; set; }
}
