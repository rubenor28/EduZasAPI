using Application.DAOs;
using Application.DTOs.Tests;
using Application.UseCases.Common;

namespace Application.UseCases.Tests;

/// <summary>
/// Caso de uso para consultar resúmenes de evaluaciones.
/// </summary>
public sealed class QueryTestSummaryUseCase(IQuerierAsync<TestSummary, TestCriteriaDTO> querier)
    : QueryUseCase<TestCriteriaDTO, TestSummary>(querier);
