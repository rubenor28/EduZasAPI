namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class ResourceEF
{
    public ulong ResourceId { get; set; }

    public bool? Active { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public ulong ProfessorId { get; set; }

    public virtual UserEF Professor { get; set; } = null!;
}
