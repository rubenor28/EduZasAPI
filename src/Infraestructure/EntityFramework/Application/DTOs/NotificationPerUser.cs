namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de relación Notificación-Usuario.
/// </summary>
public partial class NotificationPerUser
{
    public ulong NotificationId { get; set; }

    public ulong UserId { get; set; }

    public bool Readed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual Notification Notification { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
