using Application.DAOs;
using Application.DTOs.Resources;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Resources;

public sealed class ResourceQueryUseCase(
    IQuerierAsync<ResourceSummary, ResourceCriteriaDTO> querier,
    IBusinessValidationService<ResourceCriteriaDTO>? validator = null
) : QueryUseCase<ResourceCriteriaDTO, ResourceSummary>(querier, validator);
