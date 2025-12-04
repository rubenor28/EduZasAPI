namespace Domain.Entities;

/// <summary>
/// Representa la relación entre una clase y un profesor en el dominio.
/// </summary>
/// <remarks>
/// Este registro sellado modela la asignación de un profesor a una clase,
/// incluyendo si es el propietario y la fecha de creación de la relación.
/// Al ser un registro inmutable, sus propiedades solo pueden ser asignadas durante la inicialización.
/// </remarks>
public sealed record ClassProfessorDomain
{
    /// <summary>
    /// Id único del usuario.
    /// </summary>
    public required ulong UserId { get; set; }

    /// <summary>
    /// Id único de una clase.
    /// </summary>
    public required string ClassId { get; set; }

    /// <summary>
    /// Obtiene un valor que indica si el profesor es el propietario de la clase.
    /// </summary>
    /// <value>
    /// <c>true</c> si el profesor es el propietario; de lo contrario, <c>false</c>.
    /// </value>
    public required bool IsOwner { get; init; }

    /// <summary>
    /// Obtiene la fecha y hora de creación de la relación.
    /// </summary>
    /// <value>
    /// La fecha y hora en que se estableció la relación entre el profesor y la clase.
    /// </value>
    public required DateTime CreatedAt { get; init; }
}
