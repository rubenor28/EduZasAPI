using Application.DAOs;
using Application.DTOs.ClassProfessors;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.ClassProfessors;

public sealed class SearchClassProfessorUseCase(
    IQuerierAsync<ClassProfessorDomain, ClassProfessorCriteriaDTO> querier,
    IBusinessValidationService<ClassProfessorCriteriaDTO>? validator = null
) : QueryUseCase<ClassProfessorCriteriaDTO, ClassProfessorDomain>(querier, validator);
