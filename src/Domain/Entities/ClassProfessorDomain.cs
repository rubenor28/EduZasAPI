namespace Domain.Entities;

/// <summary>
/// Representa la relación entre una clase y un profesor.
/// </summary>
/// <remarks>
/// Modela la asignación de un profesor a una clase. Las propiedades <see cref="UserId"/>
/// y <see cref="ClassId"/> juntas forman la clave primaria de esta entidad.
/// </remarks>
public sealed record ClassProfessorDomain
{
    /// <summary>
    /// Obtiene o establece el identificador único del usuario (profesor).
    /// </summary>
    public required ulong UserId { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador único de la clase.
    /// </summary>
    public required string ClassId { get; set; }

    /// <summary>
    /// Obtiene un valor que indica si el profesor es el propietario de la clase.
    /// </summary>
    public required bool IsOwner { get; init; }

    /// <summary>
    /// Obtiene la fecha y hora de creación de la relación.
    /// </summary>
    public required DateTime CreatedAt { get; init; }
}
