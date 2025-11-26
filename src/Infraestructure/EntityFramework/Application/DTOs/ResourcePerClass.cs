namespace EntityFramework.Application.DTOs;

public partial class ResourcePerClass
{
    public string ClassId { get; set; } = null!;

    public Guid ResourceId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Resource Resource { get; set; } = null!;
}
