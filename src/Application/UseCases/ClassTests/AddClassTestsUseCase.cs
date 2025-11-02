using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
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
    IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> professorReader,
    IBusinessValidationService<NewClassTestDTO>? validator = null
) : AddUseCase<NewClassTestDTO, ClassTestDomain>(creator, validator)
{
    private readonly IReaderAsync<ulong, TestDomain> _testReader = testReader;
    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;
    private readonly IReaderAsync<ClassTestIdDTO, ClassTestDomain> _classTestReader =
        classTestReader;
    private readonly IReaderAsync<
        ClassUserRelationIdDTO,
        ProfessorClassRelationDTO
    > _professorReader = professorReader;

    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        NewClassTestDTO value
    )
    {
        List<FieldErrorDTO> errors = [];

        var classSearch = await _classReader.GetAsync(value.ClassId);
        if (classSearch.IsNone)
            errors.Add(new() { Field = "classId", Message = "No se encontró la clase" });

        var testSearch = await _testReader.GetAsync(value.TestId);
        if (testSearch.IsNone)
            errors.Add(new() { Field = "testId", Message = "No se encontró el test" });

        if (testSearch.IsSome)
        {
            var record = testSearch.Unwrap();
            var professorSearch = await _professorReader.GetAsync(
                new() { ClassId = value.ClassId, UserId = record.ProfesorId }
            );

            if (professorSearch.IsNone)
                errors.Add(
                    new() { Field = "testId", Message = "El dueño debe ser profesor de la clase" }
                );
        }

        if (errors.Count > 0)
            return UseCaseError.Input(errors);

        var test = testSearch.Unwrap();

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => test.ProfesorId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseError.Unauthorized();

        var classTestSearch = await _classTestReader.GetAsync(
            new() { ClassId = value.ClassId, TestId = value.TestId }
        );

        if (classTestSearch.IsSome)
            return UseCaseError.AlreadyExists();

        return Unit.Value;
    }
}
