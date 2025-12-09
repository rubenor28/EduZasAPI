namespace Domain.Entities;

/// <summary>
/// Representa la asignación de un examen (`Test`) a una clase (`Class`) específica.
/// </summary>
/// <remarks>
/// Esta entidad actúa como una tabla de unión entre `TestDomain` y `ClassDomain`,
/// permitiendo que un mismo examen sea asignado a múltiples clases y registrando
/// metadatos específicos de esa asignación, como la visibilidad.
/// </remarks>
public sealed class ClassTestDomain
{
    /// <summary>
    /// Obtiene o establece el identificador único del examen asignado.
    /// </summary>
    public required Guid TestId { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador único de la clase a la que se asigna el examen.
    /// </summary>
    public required string ClassId { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica si el examen es visible para los estudiantes de esta clase.
    /// </summary>
    public required bool Visible { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación de la asignación.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}
