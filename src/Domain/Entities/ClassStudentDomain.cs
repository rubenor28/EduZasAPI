namespace Domain.Entities;

/// <summary>
/// Representa la relación entre un estudiante y una clase a la que está inscrito.
/// </summary>
/// <remarks>
/// Esta entidad modela la inscripción de un usuario (estudiante) en una clase.
/// </remarks>
public sealed record ClassStudentDomain
{
    /// <summary>
    /// Obtiene o establece el identificador compuesto de la relación.
    /// </summary>
    /// <remarks>
    /// Contiene las claves foráneas <see cref="UserClassRelationId.UserId"/> y <see cref="UserClassRelationId.ClassId"/>
    /// que juntas forman la clave primaria de esta entidad.
    /// </remarks>
    public required UserClassRelationId Id { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica si la relación está oculta (ej. para el profesor).
    /// </summary>
    public required bool Hidden { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la inscripción.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}
