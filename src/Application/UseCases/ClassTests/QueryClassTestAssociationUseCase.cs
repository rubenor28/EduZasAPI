using Application.DAOs;
using Application.DTOs.ClassTests;
using Application.UseCases.Common;

namespace Application.UseCases.ClassTests;

public sealed class QueryClassTestAssociationUseCase(IQuerierAsync<ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO> querier)
    : QueryUseCase<ClassTestAssociationCriteriaDTO, ClassTestAssociationDTO>(querier);
