
namespace Application.DTOs.Tests;

public sealed record TestUpdateDTO
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public uint? TimeLimitMinutes { get; init; } 
    public required ulong ProfessorId { get; init; }
}
