namespace Domain.Entities;

/// <summary>
/// Representa el identificador compuesto para la asignación de una etiqueta a un contacto.
/// </summary>
public sealed record ContactTagIdDTO
{
    /// <summary>
    /// Obtiene o establece el texto de la etiqueta.
    /// </summary>
    public required ulong TagId { get; set; }

    /// <summary>
    /// Obtiene o establece el ID del propietario de la agenda.
    /// </summary>
    public required ulong AgendaOwnerId { get; set; }

    /// <summary>
    /// Obtiene o establece el ID del usuario que es el contacto.
    /// </summary>
    public required ulong UserId { get; set; }
}

/// <summary>
/// Representa la asignación de una etiqueta a un contacto en la agenda de un usuario.
/// </summary>
/// <remarks>
/// Esta entidad es una tabla de unión que vincula una etiqueta (<see cref="TagDomain"/>)
/// con un contacto (<see cref="ContactDomain"/>).
/// </remarks>
public sealed record ContactTagDomain
{
    /// <summary>
    /// Obtiene o establece el identificador compuesto de la asignación de etiqueta.
    /// </summary>
    public required ContactTagIdDTO Id { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora en que se asignó la etiqueta.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }
}
