using Application.DAOs;
using Application.DTOs.Classes;
using Application.UseCases.Common;

namespace Application.UseCases.Classes;

/// <summary>
/// Caso de uso para consultar clases con filtros.
/// </summary>
public sealed class QueryStudentClassesSummaryUseCase(
    IQuerierAsync<StudentClassesSummaryDTO, StudentClassesSummaryCriteriaDTO> querier
) : QueryUseCase<StudentClassesSummaryCriteriaDTO, StudentClassesSummaryDTO>(querier, null);
