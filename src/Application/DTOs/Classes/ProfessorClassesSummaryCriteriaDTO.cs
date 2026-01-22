using Domain.ValueObjects;

namespace Application.DTOs.Classes;

public record ProfessorClassesSummaryCriteriaDTO(
    bool? Active,
    StringQueryDTO? ClassName,
    StringQueryDTO? Subject,
    StringQueryDTO? Section,
    ulong ProfessorId
) : CriteriaDTO;
