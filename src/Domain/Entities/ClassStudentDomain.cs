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
    /// Obtiene o establece el identificador del usuario en la relación.
    /// </summary>
    public required ulong UserId { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador de la clase en la relación.
    /// </summary>
    public required string ClassId { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica si la relación está oculta (ej. para el profesor).
    /// </summary>
    public required bool Hidden { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la inscripción.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }
}
