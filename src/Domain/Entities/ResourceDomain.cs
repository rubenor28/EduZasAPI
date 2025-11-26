namespace Domain.Entities;

public sealed record ResourceDomain
{
    public required Guid Id { get; set; }
    public required bool Active { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required ulong ProfessorId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ModifiedAt { get; set; }
}
