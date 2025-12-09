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
/// Caso de uso para registrar nuevos usuarios.
/// </summary>
/// <remarks>
/// Aplica reglas de negocio específicas: verificación de unicidad de email y hashing de contraseña.
/// </remarks>
public class AddUserUseCase(
    IHashService hasher,
    ICreatorAsync<UserDomain, NewUserDTO> creator,
    IBusinessValidationService<NewUserDTO> validator,
    IReaderAsync<string, UserDomain> reader
) : AddUseCase<NewUserDTO, UserDomain>(creator, validator)
{
    private readonly IReaderAsync<string, UserDomain> _reader = reader;
    private readonly IHashService _hasher = hasher;

    /// <inheritdoc/>
    protected override Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<NewUserDTO> newEntity
    )
    {
        if (newEntity.Executor.Role != UserType.ADMIN)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NewUserDTO> request
    )
    {
        var user = await _reader.GetAsync(request.Data.Email);
        if (user is not null)
            return UseCaseErrors.Conflict("El recurso ya existe");

        return Result<Unit, UseCaseError>.Ok(Unit.Value);
    }

    /// <inheritdoc/>
    protected override UserActionDTO<NewUserDTO> PreValidationFormat(UserActionDTO<NewUserDTO> value)
    {
        return value with
        {
            Data = value.Data with
            {
                FirstName = value.Data.FirstName.ToUpperInvariant(),
                FatherLastname = value.Data.FatherLastname.ToUpperInvariant(),
                MidName = value.Data.MidName?.ToUpperInvariant(),
                MotherLastname = value.Data.MotherLastname?.ToUpperInvariant()
            }
        };
    }

    /// <inheritdoc/>
    protected override UserActionDTO<NewUserDTO> PostValidationFormat(UserActionDTO<NewUserDTO> request)
    {
        return request with { Data = request.Data with { Password = _hasher.Hash(request.Data.Password) } };
    }
}
