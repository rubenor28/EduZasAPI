using Application.DAOs;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.ClassProfessors;

public sealed class ReadClassProfessorUseCase(
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> reader
) : ReadUseCase<UserClassRelationId, ClassProfessorDomain>(reader, null);
