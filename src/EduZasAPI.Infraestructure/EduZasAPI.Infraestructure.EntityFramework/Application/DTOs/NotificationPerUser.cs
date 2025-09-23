using EduZasAPI.Infraestructure.EntityFramework.Application.Notifications;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.NotificationsPerUser;

public partial class NotificationPerUser
{
    public ulong NotificationId { get; set; }

    public ulong UserId { get; set; }

    public bool Readed { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual Notification Notification { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
