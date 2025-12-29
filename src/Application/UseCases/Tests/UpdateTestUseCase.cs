using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.DTOs.Tests;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Tests;

/// <summary>
/// Caso de uso para actualizar una evaluación.
/// </summary>
public sealed class UpdateTestUseCase(
    IUpdaterAsync<TestDomain, TestUpdateDTO> updater,
    IReaderAsync<Guid, TestDomain> reader,
    IBusinessValidationService<TestUpdateDTO> validator
) : UpdateUseCase<Guid, TestUpdateDTO, TestDomain>(updater, reader, validator)
{
    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<TestUpdateDTO> value,
        TestDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Executor.Id == value.Data.ProfessorId,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <inheritdoc/>
    protected override Guid GetId(TestUpdateDTO dto) => dto.Id;
}
