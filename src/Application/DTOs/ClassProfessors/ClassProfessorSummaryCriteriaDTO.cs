using Domain.ValueObjects;

namespace Application.DTOs.ClassProfessors;

public record ClassProfessorSummaryCriteriaDTO() : CriteriaDTO
{
    public required string ClassId { get; init; }
    public required ulong ProfessorId { get; init; }
};
