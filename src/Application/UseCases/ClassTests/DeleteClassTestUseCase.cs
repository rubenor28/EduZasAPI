using Application.DAOs;
using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassTests;

public sealed class DeleteClassTestUseCase(
    IDeleterAsync<ClassTestIdDTO, ClassTestDomain> deleter,
    IReaderAsync<ClassTestIdDTO, ClassTestDomain> reader,
    IReaderAsync<ulong, TestDomain> testReader,
    IBusinessValidationService<DeleteClassTestDTO>? validator = null
) : DeleteUseCase<ClassTestIdDTO, DeleteClassTestDTO, ClassTestDomain>(deleter, reader, validator)
{
    private readonly IReaderAsync<ulong, TestDomain> _testReader = testReader;

    private async Task<bool> IsAuthorizedProfessor(
        ClassTestDomain testClassRelation,
        Executor executor
    )
    {
        var testSearch = await _testReader.GetAsync(testClassRelation.Id.TestId);
        if (testSearch.IsNone)
            throw new InvalidDataException("No se encontró el test de la relación");

        var test = testSearch.Unwrap();

        return test.ProfessorId == executor.Id;
    }

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        DeleteClassTestDTO value
    )
    {
        var recordSearch = await _reader.GetAsync(value.Id);

        if (recordSearch.IsNone)
            return UseCaseErrors.NotFound();

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsAuthorizedProfessor(
                recordSearch.Unwrap(),
                value.Executor
            ),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    protected override Task ExtraTaskAsync(
        DeleteClassTestDTO newEntity,
        ClassTestDomain createdEntity
    )
    {
        // TODO: Eliminar respuestas
        return Task.FromResult(Unit.Value);
    }
}
