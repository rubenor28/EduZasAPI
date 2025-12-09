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
/// Caso de uso para crear una nueva evaluación.
/// </summary>
public sealed class AddTestUseCase(
    ICreatorAsync<TestDomain, NewTestDTO> creator,
    IReaderAsync<ulong, UserDomain> userReader,
    IBusinessValidationService<NewTestDTO>? validator = null
) : AddUseCase<NewTestDTO, TestDomain>(creator, validator)
{
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(UserActionDTO<NewTestDTO> value)
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

        var userSearch = await _userReader.GetAsync(value.Data.ProfessorId);
        if (userSearch is null)
            return UseCaseErrors.Input([
                new() { Field = "profesorId", Message = "No se encontró el usuario" },
            ]);

        return Unit.Value;
    }
}
