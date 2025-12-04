namespace Domain.Entities;

public sealed class ClassTestDomain
{
    public required Guid TestId { get; set; }
    public required string ClassId { get; set; }
    public required bool Visible { get; set; }
    public required DateTime CreatedAt { get; set; }
}
