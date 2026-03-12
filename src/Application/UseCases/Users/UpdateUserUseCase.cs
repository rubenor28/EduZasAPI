using Application.DAOs;
using Application.DTOs.Users;
using Application.Services;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
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

    /// <summary>
    /// Formatea los nombres del usuario a mayúsculas antes de la validación.
    /// </summary>
    /// <param name="value">Datos de la actualización.</param>
    /// <returns>Acción con los datos formateados.</returns>
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

    /// <summary>
    /// Valida asíncronamente que el ejecutor esté autorizado y que la contraseña no sea idéntica a la anterior.
    /// </summary>
    /// <param name="value">Datos de la actualización.</param>
    /// <param name="record">Entidad de usuario original.</param>
    /// <returns>Resultado exitoso o error de caso de uso.</returns>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<UserUpdateDTO> value,
        UserDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => value.Data.Id == value.Executor.Id,
        };

        var pwdInput = value.Data.Password;
        if (pwdInput is not null && _hasher.Matches(pwdInput, record.Password))
            return UseCaseErrors.Input([
                new() { Field = "password", Message = "La contraseña no puede ser la misma" },
            ]);

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <summary>
    /// Hashea la nueva contraseña y asegura que el email original no sea modificado.
    /// </summary>
    /// <param name="value">Datos de la actualización.</param>
    /// <param name="original">Entidad de usuario original.</param>
    /// <returns>Acción con los datos procesados.</returns>
    protected override UserActionDTO<UserUpdateDTO> PostValidationFormat(
        UserActionDTO<UserUpdateDTO> value,
        UserDomain original
    )
    {
        var passwordFormat = value.Data.Password.Match(
            (pwd) => value with { Data = value.Data with { Password = _hasher.Hash(pwd) } },
            () => value
        );

        return passwordFormat with
        {
            // Asegurarse que no cambie el Email
            Data = passwordFormat.Data with
            {
                Email = original.Email,
            },
        };
    }

    /// <summary>
    /// Obtiene el identificador del usuario desde el DTO de actualización.
    /// </summary>
    /// <param name="dto">DTO de actualización.</param>
    /// <returns>ID del usuario.</returns>
    protected override ulong GetId(UserUpdateDTO dto) => dto.Id;
}
