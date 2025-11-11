namespace Domain.Entities;

public sealed record UserClassRelationId
{
    public required ulong UserId { get; set; }
    public required string ClassId { get; set; }
}
