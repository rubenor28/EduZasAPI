using Domain.Entities;

public sealed record AnswerUpdateStudentValidationDTO
{
    public required AnswerUpdateStudentDTO AnswerUpdate { get; set; }
    public required TestDomain Test { get; set; }
}
