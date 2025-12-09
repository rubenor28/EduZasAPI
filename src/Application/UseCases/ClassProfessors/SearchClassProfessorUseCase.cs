using Application.DAOs;
using Application.DTOs.ClassProfessors;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.ClassProfessors;

/// <summary>
/// Caso de uso para buscar profesores de clases.
/// </summary>
public sealed class SearchClassProfessorUseCase(
    IQuerierAsync<ClassProfessorDomain, ClassProfessorCriteriaDTO> querier,
    IBusinessValidationService<ClassProfessorCriteriaDTO>? validator = null
) : QueryUseCase<ClassProfessorCriteriaDTO, ClassProfessorDomain>(querier, validator);
