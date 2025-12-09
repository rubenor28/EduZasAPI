using Application.DAOs;
using Application.DTOs.ClassResources;
using Application.UseCases.Common;

namespace Application.UseCases.ClassResource;

/// <summary>
/// Caso de uso para consultar asociaciones de recursos en clases.
/// </summary>
public sealed class ClassResourceAssociationQueryUseCase(
    IQuerierAsync<ClassResourceAssociationDTO, ClassResourceAssociationCriteriaDTO> querier
) : QueryUseCase<ClassResourceAssociationCriteriaDTO, ClassResourceAssociationDTO>(querier, null);