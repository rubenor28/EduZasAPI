using Application.DAOs;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.ClassProfessors;

/// <summary>
/// Caso de uso para leer la relación de un profesor con una clase.
/// </summary>
public sealed class ReadClassProfessorUseCase(
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> reader
) : ReadUseCase<UserClassRelationId, ClassProfessorDomain>(reader, null);
