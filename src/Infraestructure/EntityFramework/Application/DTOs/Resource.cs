namespace EntityFramework.Application.DTOs;

public partial class Resource
{
    public Guid ResourceId { get; set; }

    public bool? Active { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public ulong ProfessorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual User Professor { get; set; } = null!;

    public virtual ICollection<ResourcePerClass> ResourcesPerClass { get; set; } = new List<ResourcePerClass>();
}
