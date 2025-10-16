using EduZasAPI.Infraestructure.EntityFramework.Application.AgendaContacts;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Tags;

public partial class TagsPerUser
{
    public ulong TagId { get; set; }
    public ulong ContactId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual AgendaContact Contact { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
