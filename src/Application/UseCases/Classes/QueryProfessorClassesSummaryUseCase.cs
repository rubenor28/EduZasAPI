using Application.DAOs;
using Application.DTOs.Classes;
using Application.UseCases.Common;

namespace Application.UseCases.Classes;

/// <summary>
/// Caso de uso para consultar clases con filtros.
/// </summary>
public sealed class QueryProfessorClassesSummaryUseCase(
    IQuerierAsync<ProfessorClassesSummaryDTO, ProfessorClassesSummaryCriteriaDTO> querier
) : QueryUseCase<ProfessorClassesSummaryCriteriaDTO, ProfessorClassesSummaryDTO>(querier, null);
