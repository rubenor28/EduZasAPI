using Application.DAOs;
using Application.DTOs.ClassTests;
using Application.UseCases.Common;

namespace Application.UseCases.ClassTests;

/// <summary>
/// Caso de uso para consultar asociaciones de evaluaciones en clases.
/// </summary>
public sealed class QueryClassTestAssociationUseCase(IQuerierAsync<ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO> querier)
    : QueryUseCase<ClassTestAssociationCriteriaDTO, ClassTestAssociationDTO>(querier);
