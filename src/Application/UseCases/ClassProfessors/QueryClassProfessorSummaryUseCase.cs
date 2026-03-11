using Application.DAOs;
using Application.DTOs.ClassProfessors;
using Application.UseCases.Common;

namespace Application.UseCases.ClassProfessors;

/// <summary>
/// Caso de uso para consultar un resumen de profesores asociados a una clase.
/// </summary>
public sealed class QueryClassProfessorSummaryUseCase(
    IQuerierAsync<ClassProfessorSummaryDTO, ClassProfessorSummaryCriteriaDTO> querier
) : QueryUseCase<ClassProfessorSummaryCriteriaDTO, ClassProfessorSummaryDTO>(querier, null);
