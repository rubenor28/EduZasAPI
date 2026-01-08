using Application.DAOs;
using Application.DTOs;
using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassTests;

/// <summary>
/// Caso de uso para actualizar la asociación de una evaluación con una clase.
/// </summary>
public sealed class UpdateClassTestUseCase(
    IUpdaterAsync<ClassTestDomain, ClassTestDTO> updater,
    IReaderAsync<ClassTestIdDTO, ClassTestDomain> reader,
    IReaderAsync<Guid, TestDomain> testReader,
    IBusinessValidationService<ClassTestDTO>? validator = null
) : UpdateUseCase<ClassTestIdDTO, ClassTestDTO, ClassTestDomain>(updater, reader, validator)
{
    private readonly IReaderAsync<Guid, TestDomain> _testReader = testReader;

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassTestDTO> value,
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

    private async Task<bool> IsAuthorizedProfessor(
        ClassTestDomain testClassRelation,
        Executor executor
    )
    {
        var test =
            await _testReader.GetAsync(testClassRelation.TestId)
            ?? throw new InvalidDataException("No se encontró el test de la relación");

        return test.ProfessorId == executor.Id;
    }

    /// <inheritdoc/>
    protected override ClassTestIdDTO GetId(ClassTestDTO dto) =>
        new() { ClassId = dto.ClassId, TestId = dto.TestId };
}
