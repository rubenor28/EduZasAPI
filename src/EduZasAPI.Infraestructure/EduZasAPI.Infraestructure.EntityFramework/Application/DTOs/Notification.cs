namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class NotificationEF
{
    public ulong NotificationId { get; set; }

    public bool? Active { get; set; }

    public string Title { get; set; } = null!;

    public string ClassId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ClassEF Class { get; set; } = null!;

    public virtual ICollection<NotificationPerUserEF> NotificationPerUsers { get; set; } = new List<NotificationPerUserEF>();
}
