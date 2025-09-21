namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class NotificationPerUserEF
{
    public ulong NotificationId { get; set; }

    public ulong UserId { get; set; }

    public bool Readed { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual NotificationEF Notification { get; set; } = null!;

    public virtual UserEF User { get; set; } = null!;
}
