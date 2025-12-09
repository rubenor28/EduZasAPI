using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Users;

/// <summary>
/// Caso de uso para actualizar información de usuarios.
/// </summary>
/// <remarks>
/// Normaliza nombres a mayúsculas y restringe la operación a administradores.
/// </remarks>
public sealed class UpdateUserUseCase(
    IUpdaterAsync<UserDomain, UserUpdateDTO> updater,
    IReaderAsync<ulong, UserDomain> reader,
    IHashService hasher,
    IBusinessValidationService<UserUpdateDTO>? validator = null
) : UpdateUseCase<ulong, UserUpdateDTO, UserDomain>(updater, reader, validator)
{
    private readonly IHashService _hasher = hasher;

    /// <inheritdoc/>
    protected override UserActionDTO<UserUpdateDTO> PreValidationFormat(
        UserActionDTO<UserUpdateDTO> value
    ) =>
        new()
        {
            Data = value.Data with
            {
                FatherLastname = value.Data.FatherLastname.ToUpperInvariant(),
                MotherLastname = value.Data.MotherLastname?.ToUpperInvariant(),
                FirstName = value.Data.FirstName.ToUpperInvariant(),
                MidName = value.Data.MidName?.ToUpperInvariant(),
            },
            Executor = value.Executor,
        };

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<UserUpdateDTO> value,
        UserDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => false,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <inheritdoc/>
    protected override UserActionDTO<UserUpdateDTO> PostValidationFormat(
        UserActionDTO<UserUpdateDTO> value,
        UserDomain original
    )
    {
        if (!_hasher.Matches(value.Data.Password!, original.Password))
        {
            return value with
            {
                Data = value.Data with { Password = _hasher.Hash(value.Data.Password!) },
            };
        }
        else
        {
            return value with { Data = value.Data with { Password = original.Password } };
        }
    }

    /// <inheritdoc/>
    protected override ulong GetId(UserUpdateDTO dto) => dto.Id;
}
