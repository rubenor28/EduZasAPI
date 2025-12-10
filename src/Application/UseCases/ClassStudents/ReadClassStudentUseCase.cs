using Application.DAOs;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.ClassStudents;

/// <summary>
/// Caso de uso para leer la relación de un estudiante con una clase.
/// </summary>
public sealed class ReadClassStudentUseCase(
    IReaderAsync<UserClassRelationId, ClassStudentDomain> reader
) : ReadUseCase<UserClassRelationId, ClassStudentDomain>(reader, null);
