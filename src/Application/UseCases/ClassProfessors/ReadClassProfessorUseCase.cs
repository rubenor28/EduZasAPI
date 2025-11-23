using Application.DAOs;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.ClassProfessors;

public sealed class ReadClasspProfessorUseCase(
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> reader,
    IBusinessValidationService<UserClassRelationId> validator
) : ReadUseCase<UserClassRelationId, ClassProfessorDomain>(reader, validator);
