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
/// Caso de uso específico para la adición de nuevos usuarios al sistema.
/// </summary>
/// <remarks>
/// Esta clase extiende la funcionalidad base de <see cref="AddUseCase{NE, E}"/> para aplicar
/// validaciones y transformaciones específicas para usuarios, como la verificación de unicidad
/// de email y el hashing de contraseñas.
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

    protected override Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<NewUserDTO> newEntity
    )
    {
        if (newEntity.Executor.Role != UserType.ADMIN)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <summary>
    /// Realiza validación asíncrona adicional para verificar la unicidad del email.
    /// </summary>
    /// <param name="request.Data">DTO con los datos del nuevo usuario a validar.</param>
    /// <returns>
    /// Un resultado que indica si la validación fue exitosa o contiene errores de email duplicado.
    /// </returns>
    /// <exception cref="InvalidDataException">
    /// Se lanza cuando se detecta más de un usuario con el mismo email en la base de datos.
    /// </exception>
    /// <remarks>
    /// Este método verifica que no exista otro usuario con el mismo email en el sistema.
    /// Si encuentra exactamente un usuario con el mismo email, retorna un error de validación.
    /// Si encuentra más de uno, lanza una excepción indicando inconsistencia en los datos.
    /// </remarks>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NewUserDTO> request
    )
    {
        var user = await _reader.GetAsync(request.Data.Email);
        if (user is not null)
            return UseCaseErrors.Conflict("El recurso ya existe");

        return Result<Unit, UseCaseError>.Ok(Unit.Value);
    }

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

    /// <summary>
    /// Aplica formato final a los datos del usuario antes de la persistencia.
    /// </summary>
    /// <param name="u">DTO con los datos del usuario a formatear.</param>
    /// <returns>DTO con los datos formateados, incluyendo la contraseña hasheada.</returns>
    /// <remarks>
    /// Este método se encarga de aplicar el hashing a la contraseña del usuario
    /// antes de que sea persistida en la base de datos.
    /// </remarks>
    protected override UserActionDTO<NewUserDTO> PostValidationFormat(UserActionDTO<NewUserDTO> request)
    {
        return request with { Data = request.Data with { Password = _hasher.Hash(request.Data.Password) } };
    }
}
