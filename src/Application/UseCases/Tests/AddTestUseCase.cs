using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Tests;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Tests;

public sealed class AddTestUseCase(
    ICreatorAsync<TestDomain, NewTestDTO> creator,
    IReaderAsync<ulong, UserDomain> userReader,
    IBusinessValidationService<NewTestDTO>? validator = null
) : AddUseCase<NewTestDTO, TestDomain>(creator, validator)
{
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;

    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        NewTestDTO value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Executor.Id == value.ProfesorId,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseError.Unauthorized();

        var userSearch = await _userReader.GetAsync(value.ProfesorId);
        if (userSearch.IsNone)
            return UseCaseError.Input(
                [new() { Field = "profesorId", Message = "No se encontró el usuario" }]
            );

        return Unit.Value;
    }
}
