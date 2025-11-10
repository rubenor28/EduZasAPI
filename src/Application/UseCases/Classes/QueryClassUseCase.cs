using Application.DAOs;
using Application.DTOs.Classes;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Classes;

public sealed class QueryClassUseCase(
    IQuerierAsync<ClassDomain, ClassCriteriaDTO> querier,
    IBusinessValidationService<ClassCriteriaDTO>? validator = null
) : QueryUseCase<ClassCriteriaDTO, ClassDomain>(querier, validator);
