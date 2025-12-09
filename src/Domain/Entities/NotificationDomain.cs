namespace Domain.Entities;

/// <summary>
/// Representa una notificación general asociada a una clase.
/// </summary>
/// <remarks>
/// Las notificaciones se crean a nivel de clase y luego se distribuyen a los usuarios
/// a través de la entidad <see cref="UserNotificationDomain"/>.
/// </remarks>
public sealed record NotificationDomain
{
    /// <summary>
    /// Obtiene o establece el identificador único de la notificación.
    /// </summary>
    public required ulong Id { get; set; }

    /// <summary>
    /// Obtiene o establece si la notificación está activa.
    /// </summary>
    public required bool Active { get; set; }

    /// <summary>
    /// Obtiene o establece el título de la notificación.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Obtiene o establece el ID de la clase a la que pertenece la notificación.
    /// </summary>
    public required string ClassId { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación de la notificación.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}
