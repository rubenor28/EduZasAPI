using Application.DAOs;
using Application.DTOs.Classes;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Classes;

/// <summary>
/// Caso de uso para consultar clases con filtros.
/// </summary>

public sealed class QueryClassUseCase(
    IQuerierAsync<ClassDomain, ClassCriteriaDTO> querier,
    IBusinessValidationService<ClassCriteriaDTO>? validator = null
) : QueryUseCase<ClassCriteriaDTO, ClassDomain>(querier, validator);
