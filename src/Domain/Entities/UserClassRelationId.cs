namespace Domain.Entities;

/// <summary>
/// Representa el identificador compuesto para una relación entre un usuario y una clase.
/// </summary>
/// <remarks>
/// Este objeto de valor se utiliza como clave primaria en entidades que modelan
/// una relación directa entre un <see cref="UserDomain"/> y una <see cref="ClassDomain"/>,
/// como <see cref="ClassStudentDomain"/>.
/// </remarks>
public sealed record UserClassRelationId
{
    /// <summary>
    /// Obtiene o establece el identificador del usuario en la relación.
    /// </summary>
    public required ulong UserId { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador de la clase en la relación.
    /// </summary>
    public required string ClassId { get; set; }
}
