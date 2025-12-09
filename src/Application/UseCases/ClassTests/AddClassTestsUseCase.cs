using Application.DAOs;
using Application.DTOs;
using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Domain.ValueObjects;

namespace Application.UseCases.ClassTests;

/// <summary>
/// Caso de uso para asociar una evaluación a una clase.
/// </summary>
public sealed class AddClassTestUseCase(
    ICreatorAsync<ClassTestDomain, ClassTestDTO> creator,
    IReaderAsync<Guid, TestDomain> testReader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<ClassTestIdDTO, ClassTestDomain> classTestReader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader,
    IBusinessValidationService<ClassTestDTO>? validator = null
) : AddUseCase<ClassTestDTO, ClassTestDomain>(creator, validator)
{
    private readonly IReaderAsync<Guid, TestDomain> _testReader = testReader;
    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;
    private readonly IReaderAsync<ClassTestIdDTO, ClassTestDomain> _classTestReader =
        classTestReader;
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassTestDTO> value
    )
    {
        List<FieldErrorDTO> errors = [];

        (await _classReader.GetAsync(value.Data.ClassId)).IfNull(() =>
            errors.Add(new() { Field = "classId", Message = "No se encontró la clase" })
        );

        var test = await _testReader.GetAsync(value.Data.TestId);

        test.IfNull(() =>
            errors.Add(new() { Field = "testId", Message = "No se encontró el test" })
        );

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => await TestOwnerIsClassProfessor(test!.ProfessorId, value.Data.ClassId),
            UserType.PROFESSOR => await IsProfessorAuthorized(
                value.Executor.Id,
                value.Data.ClassId,
                test!
            ),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var classTestSearch = await _classTestReader.GetAsync(
            new() { ClassId = value.Data.ClassId, TestId = value.Data.TestId }
        );

        if (classTestSearch is not null)
            return UseCaseErrors.Conflict("El recurso ya existe");

        return Unit.Value;
    }

    private async Task<bool> TestOwnerIsClassProfessor(ulong testOwner, string classId)
    {
        var professor = await _professorReader.GetAsync(
            new() { ClassId = classId, UserId = testOwner }
        );

        return professor is not null;
    }

    private async Task<bool> IsProfessorAuthorized(
        ulong professorId,
        string classId,
        TestDomain test
    )
    {
        var professorSearch = await _professorReader.GetAsync(
            new() { ClassId = classId, UserId = professorId }
        );

        return professorSearch is not null && test.ProfessorId == professorId;
    }
}
