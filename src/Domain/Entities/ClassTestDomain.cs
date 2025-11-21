namespace Domain.Entities;

public sealed record ClassTestIdDTO
{
    public required ulong TestId { get; set; }
    public required string ClassId { get; set; }
}

public sealed class ClassTestDomain
{
    public required ClassTestIdDTO Id { get; set; }
    public required bool Visible { get; set; }
    public required DateTime CreatedAt { get; set; }
}
