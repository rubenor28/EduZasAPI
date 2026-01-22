using Application.DTOs.ClassTests;
using Domain.Entities;

public record ClassTestUpdateDTO
{
    /// <summary>ID de la evaluación.</summary>
    public required Guid TestId { get; init; }

    /// <summary>ID de la clase.</summary>
    public required string ClassId { get; init; }

    public bool? AllowModifyAnswers { get; init; }

    public ClassTestUpdateDTO() { }

    public ClassTestUpdateDTO(ClassTestIdDTO id)
    {
        TestId = id.TestId;
        ClassId = id.ClassId;
    }

    public ClassTestUpdateDTO(ClassTestDomain value)
    {
        TestId = value.TestId;
        ClassId = value.ClassId;
        AllowModifyAnswers = value.AllowModifyAnswers;
    }
}
