namespace Application.DTOs.Tests;

public sealed record TestSummary
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required bool Active { get; set; }
    public required DateTime ModifiedAt { get; set; }
}
