using Application.DAOs;
using Application.DTOs.ClassResources;
using Application.UseCases.Common;

namespace Application.UseCases.ClassResource;

public sealed class ClassResourceAssociationQueryUseCase(
    IQuerierAsync<ClassResourceAssociationDTO, ClassResourceAssociationCriteriaDTO> querier
) : QueryUseCase<ClassResourceAssociationCriteriaDTO, ClassResourceAssociationDTO>(querier, null);