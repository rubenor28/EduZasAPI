namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de relación Clase-Recurso.
/// </summary>
public partial class ClassResource
{
    public string ClassId { get; set; } = null!;

    public Guid ResourceId { get; set; }

    public bool Hidden { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Resource Resource { get; set; } = null!;
}
