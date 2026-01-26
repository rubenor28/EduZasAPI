namespace Domain.Entities;

/// <summary>
/// Representa la metadata de uso de un recurso por un usuario.
/// </summary>
/// <remarks>
/// Se utiliza para recopilar cuánto tiempo pasa un usuario viendo un recurso,
/// con el fin de generar reportes y análisis de participación.
/// </remarks>
public sealed record ResourceViewSessionDomain
{
    /// <summary>
    /// Obtiene o establece el identificador único de la sesión de visualización.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Obtiene o establece el ID del usuario que visualiza el recurso.
    /// </summary>
    public required ulong UserId { get; set; }

    /// <summary>
    /// Obtiene o establece el ID del recurso que está siendo visualizado.
    /// </summary>
    public required Guid ResourceId { get; set; }

    /// <summary>
    /// Obtiene o establece el ID de la clase en el contexto de la cual se visualiza el recurso.
    /// </summary>
    public required string ClassId { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora (UTC) de inicio de la visualización.
    /// </summary>
    public required DateTimeOffset StartTimeUtc { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora (UTC) de finalización de la visualización. Es nulo si la sesión sigue activa.
    /// </summary>
    public required DateTimeOffset? EndTimeUtc { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación del registro de la sesión.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la última modificación del registro.
    /// </summary>
    public required DateTimeOffset ModifiedAt { get; set; }
}
