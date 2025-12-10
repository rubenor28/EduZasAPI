using Application.DTOs.Common;

namespace Application.DTOs.Classes;

public record StudentClassesSummaryCriteriaDTO(
    bool? Active,
    StringQueryDTO? ClassName,
    StringQueryDTO? Subject,
    StringQueryDTO? Section,
    bool? Hidden,
    ulong StudentId
) : CriteriaDTO;
