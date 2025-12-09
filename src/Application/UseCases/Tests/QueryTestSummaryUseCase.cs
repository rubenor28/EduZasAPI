using Application.DAOs;
using Application.DTOs.Tests;
using Application.UseCases.Common;

namespace Application.UseCases.Tests;

public sealed class QueryTestSummaryUseCase(IQuerierAsync<TestSummary, TestCriteriaDTO> querier)
    : QueryUseCase<TestCriteriaDTO, TestSummary>(querier);
