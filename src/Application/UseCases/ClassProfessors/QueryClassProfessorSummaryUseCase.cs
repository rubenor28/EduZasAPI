using Application.DAOs;
using Application.DTOs.ClassProfessors;
using Application.UseCases.Common;

namespace Application.UseCases.ClassProfessors;

public sealed class QueryClassProfessorSummaryUseCase(
    IQuerierAsync<ClassProfessorSummaryDTO, ClassProfessorSummaryCriteriaDTO> querier
) : QueryUseCase<ClassProfessorSummaryCriteriaDTO, ClassProfessorSummaryDTO>(querier, null);
