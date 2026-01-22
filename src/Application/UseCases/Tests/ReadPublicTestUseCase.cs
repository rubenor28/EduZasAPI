using Application.DAOs;
using Application.DTOs.ClassTests;
using Application.DTOs.Tests;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

public class ReadPublicTestUseCase(
    IReaderAsync<PublicTestIdDTO, PublicTestDTO> reader,
    IReaderAsync<ClassTestIdDTO, ClassTestDomain> classTestReader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> classProfessorReader,
    IReaderAsync<UserClassRelationId, ClassStudentDomain> classStudentReader
) : ReadUseCase<PublicTestIdDTO, PublicTestDTO>(reader, null)
{
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _classProfessorReader =
        classProfessorReader;
    private readonly IReaderAsync<UserClassRelationId, ClassStudentDomain> _classStudentReader =
        classStudentReader;
    private readonly IReaderAsync<ClassTestIdDTO, ClassTestDomain> _classTestReader =
        classTestReader;

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<PublicTestIdDTO> value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(value),
            UserType.STUDENT => await IsStudentAuthorized(value),
            _ => throw new NotSupportedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    private async Task<bool> IsProfessorAuthorized(UserActionDTO<PublicTestIdDTO> value)
    {
        var executorId = value.Executor.Id;
        var classId = value.Data.ClassId;

        // Si el test no está asignado a la clase no autorizamos
        var classTest = await _classTestReader.GetAsync(
            new() { ClassId = classId, TestId = value.Data.TestId }
        );

        if (classTest is null)
            return false;

        // Verificar que el profesor sea profesor de la clase
        var classProfessor = await _classProfessorReader.GetAsync(
            new() { UserId = executorId, ClassId = classId }
        );

        if (classProfessor is not null)
            return true;

        // O en su defecto que sea estudiante de la clase
        var classStudent = await _classStudentReader.GetAsync(
            new() { UserId = executorId, ClassId = classId }
        );

        if (classStudent is not null)
            return true;

        // Si no pertenece a la clase, no permitimos que lea el test
        return false;
    }

    private async Task<bool> IsStudentAuthorized(UserActionDTO<PublicTestIdDTO> value)
    {
        var executorId = value.Executor.Id;
        var classId = value.Data.ClassId;

        // Si el test no está asignado a la clase no autorizamos
        var classTest = await _classTestReader.GetAsync(
            new() { ClassId = classId, TestId = value.Data.TestId }
        );

        if (classTest is null)
            return false;

        // Verificar que el usuario sea estudiante de la clase
        var classStudent = await _classStudentReader.GetAsync(
            new() { UserId = executorId, ClassId = classId }
        );

        if (classStudent is not null)
            return true;

        // Si no pertenece a la clase, no permitimos que lea el test
        return false;
    }
}
