using Application.DAOs;
using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassTests;

public sealed class AddClassTestUseCase(
    ICreatorAsync<ClassTestDomain, NewClassTestDTO> creator,
    IReaderAsync<ulong, TestDomain> testReader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<ClassTestIdDTO, ClassTestDomain> classTestReader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader,
    IBusinessValidationService<NewClassTestDTO>? validator = null
) : AddUseCase<NewClassTestDTO, ClassTestDomain>(creator, validator)
{
    private readonly IReaderAsync<ulong, TestDomain> _testReader = testReader;
    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;
    private readonly IReaderAsync<ClassTestIdDTO, ClassTestDomain> _classTestReader =
        classTestReader;
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        NewClassTestDTO value
    )
    {
        List<FieldErrorDTO> errors = [];

        (await _classReader.GetAsync(value.ClassId)).IfNone(() =>
            errors.Add(new() { Field = "classId", Message = "No se encontró la clase" })
        );

        var testSearch = await _testReader.GetAsync(value.TestId);

        testSearch.IfNone(() =>
            errors.Add(new() { Field = "testId", Message = "No se encontró el test" })
        );

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => await TestOwnerIsClassProfessor(
                testSearch.Unwrap().ProfessorId,
                value.ClassId
            ),
            UserType.PROFESSOR => await IsProfessorAuthorized(
                value.Executor.Id,
                value.ClassId,
                testSearch.Unwrap()
            ),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var classTestSearch = await _classTestReader.GetAsync(
            new() { ClassId = value.ClassId, TestId = value.TestId }
        );

        if (classTestSearch.IsSome)
            return UseCaseErrors.AlreadyExists();

        return Unit.Value;
    }

    private async Task<bool> TestOwnerIsClassProfessor(ulong testOwner, string classId)
    {
        var professor = await _professorReader.GetAsync(
            new() { ClassId = classId, UserId = testOwner }
        );

        return professor.IsSome;
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

        return professorSearch.IsSome && test.ProfessorId == professorId;
    }
}
