using EduZasAPI.Infraestructure.EntityFramework.Application.Users;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Resources;

public partial class Resource
{
    public ulong ResourceId { get; set; }

    public bool? Active { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public ulong ProfessorId { get; set; }

    public virtual User Professor { get; set; } = null!;
}
