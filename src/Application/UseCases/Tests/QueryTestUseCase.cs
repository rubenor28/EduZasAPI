using Application.DAOs;
using Application.DTOs.Tests;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Tests;

public sealed class QueryTestUseCase(IQuerierAsync<TestDomain, TestCriteriaDTO> querier)
    : QueryUseCase<TestCriteriaDTO, TestDomain>(querier);
