namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad que representa la metadata de uso de un recurso por parte de un usuario.
/// Registra el tiempo de visualización para fines de reportes y análisis.
/// </summary>
public partial class ResourceViewSession
{
    public Guid Id { get; set; }
    public ulong UserId { get; set; }
    public Guid ResourceId { get; set; }
    public string ClassId { get; set; } = null!;
    public DateTimeOffset StartTimeUtc { get; set; }
    public DateTimeOffset EndTimeUtc { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Resource Resource { get; set; } = null!;
    public virtual Class Class { get; set; } = null!;
}
