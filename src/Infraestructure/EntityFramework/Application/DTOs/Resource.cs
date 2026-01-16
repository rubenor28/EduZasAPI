using System.Text.Json.Nodes;

namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de recurso.
/// </summary>
public partial class Resource
{
    public Guid ResourceId { get; set; }

    public bool? Active { get; set; }

    public string Title { get; set; } = null!;

    public string Color { get; set; } = null!;

    public JsonNode Content { get; set; } = null!;

    public ulong ProfessorId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public virtual User Professor { get; set; } = null!;

    public virtual ICollection<ClassResource> ClassResources { get; set; } = new List<ClassResource>();

    public virtual ICollection<ResourceViewSession> ResourceViewSessions { get; set; } = new List<ResourceViewSession>();
}
