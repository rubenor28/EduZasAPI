namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de relación Notificación-Usuario.
/// </summary>
public partial class NotificationPerUser
{
    /// <summary>
    /// ID de la notificación.
    /// </summary>
    public ulong NotificationId { get; set; }

    /// <summary>
    /// ID del usuario.
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Indica si la notificación ha sido leída.
    /// </summary>
    public bool Readed { get; set; }

    /// <summary>
    /// Fecha de creación de la relación.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Fecha de modificación de la relación.
    /// </summary>
    public DateTimeOffset ModifiedAt { get; set; }

    /// <summary>
    /// Notificación asociada.
    /// </summary>
    public virtual Notification Notification { get; set; } = null!;

    /// <summary>
    /// Usuario asociado.
    /// </summary>
    public virtual User User { get; set; } = null!;
}
