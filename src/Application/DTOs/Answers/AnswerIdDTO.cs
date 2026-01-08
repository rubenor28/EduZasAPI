namespace Application.DTOs.Answers;

public sealed record AnswerIdDTO
{
    public required ulong UserId { get; set; }
    public required Guid TestId { get; set; }
    public required string ClassId { get; set; }
}
