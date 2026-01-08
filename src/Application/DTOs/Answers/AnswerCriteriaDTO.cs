using Application.DTOs.Common;

public record AnswerCriteriaDTO : CriteriaDTO
{
    public Guid? TestId { get; set; }
    public ulong? UserId { get; set; }
    public string? ClassId { get; set; }
    public ulong? TestOwnerId { get; set; }
}
