using Application.DAOs;
using Application.DTOs.Tests;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Tests;

/// <summary>
/// Caso de uso para consultar evaluaciones completas.
/// </summary>
public sealed class QueryTestUseCase(IQuerierAsync<TestDomain, TestCriteriaDTO> querier)
    : QueryUseCase<TestCriteriaDTO, TestDomain>(querier);
