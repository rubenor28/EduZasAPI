namespace Domain.Entities;

/// <summary>
/// Representa el identificador compuesto para la relación entre un usuario y una notificación.
/// </summary>
public sealed record UserNotificationIdDTO
{
    /// <summary>
    /// Obtiene o establece el ID de la notificación.
    /// </summary>
    public required ulong NotificationId { get; set; }

    /// <summary>
    /// Obtiene o establece el ID del usuario.
    /// </summary>
    public required ulong UserId { get; set; }
}

/// <summary>
/// Representa el estado de una notificación para un usuario específico.
/// </summary>
/// <remarks>
/// Actúa como una tabla de unión entre <see cref="NotificationDomain"/> y <see cref="UserDomain"/>,
/// registrando si un usuario ha leído una notificación específica.
/// </remarks>
public sealed record UserNotificationDomain
{
    /// <summary>
    /// Obtiene o establece el identificador compuesto de la relación.
    /// </summary>
    public required UserNotificationIdDTO Id { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica si el usuario ha leído la notificación.
    /// </summary>
    public required bool Readed { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación del registro.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la última modificación (ej. cuando se marca como leída).
    /// </summary>
    public DateTimeOffset ModifiedAt { get; set; }
}
