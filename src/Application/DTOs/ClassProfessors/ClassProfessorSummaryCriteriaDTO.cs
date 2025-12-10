using Application.DTOs.Common;

namespace Application.DTOs.ClassProfessors;

public record ClassProfessorSummaryCriteriaDTO() : CriteriaDTO
{
    public required string ClassId { get; init; }
    public ulong ProfessorId { get; init; }
};
