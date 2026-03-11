namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de notificación.
/// </summary>
public partial class Notification
{
    /// <summary>
    /// ID de la notificación.
    /// </summary>
    public ulong NotificationId { get; set; }

    /// <summary>
    /// Indica si la notificación está activa.
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// Título de la notificación.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// ID de la clase a la que pertenece la notificación.
    /// </summary>
    public string ClassId { get; set; } = null!;

    /// <summary>
    /// Fecha de creación de la notificación.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Clase a la que pertenece la notificación.
    /// </summary>
    public virtual Class Class { get; set; } = null!;

    /// <summary>
    /// Notificaciones por usuario asociadas a esta notificación.
    /// </summary>
    public virtual ICollection<NotificationPerUser> NotificationPerUsers { get; set; } = new List<NotificationPerUser>();
}
