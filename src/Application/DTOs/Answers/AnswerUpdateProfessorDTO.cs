using Domain.Entities;

public record AnswerUpdateProfessorDTO
{
    public required ulong UserId { get; set; }
    public required Guid TestId { get; set; }
    public required string ClassId { get; set; }
    public required AnswerMetadata Metadata { get; set; }
}
