using Application.DAOs;
using Application.DTOs.ClassResources;
using Application.UseCases.Common;

namespace Application.UseCases.ClassResource;

public sealed class ClassResourceAssosiationQueryUseCase(
    IQuerierAsync<ClassResourceAssosiationDTO, ClassResourceAssosiationCriteriaDTO> querier
) : QueryUseCase<ClassResourceAssosiationCriteriaDTO, ClassResourceAssosiationDTO>(querier, null);
