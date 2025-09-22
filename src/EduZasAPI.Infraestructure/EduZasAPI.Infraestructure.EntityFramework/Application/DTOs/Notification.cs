using System;
using System.Collections.Generic;

namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class Notification
{
    public ulong NotificationId { get; set; }

    public bool? Active { get; set; }

    public string Title { get; set; } = null!;

    public string ClassId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<NotificationPerUser> NotificationPerUsers { get; set; } = new List<NotificationPerUser>();
}
