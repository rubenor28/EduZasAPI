using Application.DAOs;
using Application.DTOs;
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
    IReaderAsync<Guid, TestDomain> testReader,
    IBusinessValidationService<ClassTestIdDTO>? validator = null
) : DeleteUseCase<ClassTestIdDTO, ClassTestDomain>(deleter, reader, validator)
{
    private readonly IReaderAsync<Guid, TestDomain> _testReader = testReader;

    private async Task<bool> IsAuthorizedProfessor(
        ClassTestDomain testClassRelation,
        Executor executor
    )
    {
        var test = await _testReader.GetAsync(testClassRelation.TestId);
        return test is null
            ? throw new InvalidDataException("No se encontró el test de la relación")
            : test.ProfessorId == executor.Id;
    }

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassTestIdDTO> value,
        ClassTestDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsAuthorizedProfessor(record, value.Executor),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    protected override Task ExtraTaskAsync(
        UserActionDTO<ClassTestIdDTO> newEntity,
        ClassTestDomain createdEntity
    )
    {
        // TODO: Eliminar respuestas
        return Task.FromResult(Unit.Value);
    }
}
