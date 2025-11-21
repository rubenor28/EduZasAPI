using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Tests;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Tests;

public sealed class UpdateTestUseCase(
    IUpdaterAsync<TestDomain, TestUpdateDTO> updater,
    IReaderAsync<ulong, TestDomain> reader,
    IBusinessValidationService<TestUpdateDTO>? validator = null
) : UpdateUseCase<ulong, TestUpdateDTO, TestDomain>(updater, reader, validator)
{
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        TestUpdateDTO value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Executor.Id == value.ProfessorId,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var record = await _reader.GetAsync(value.Id);
        if (record.IsNone)
            return UseCaseErrors.NotFound();

        return Unit.Value;
    }

    protected override ulong GetId(TestUpdateDTO dto) => dto.Id;
}
