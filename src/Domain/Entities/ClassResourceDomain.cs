namespace Domain.Entities;

public sealed record ClassResourceDomain
{
    public required string ClassId { get; set; }
    public required Guid ResourceId { get; set; }
    public required bool Hidden { get; set; }
    public required DateTime CreatedAt { get; set; }
}
