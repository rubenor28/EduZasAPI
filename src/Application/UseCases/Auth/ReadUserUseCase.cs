using Application.DAOs;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Auth;

/// <summary>
/// Caso de uso para obtener detalles de un usuario.
/// </summary>
/// <remarks>
/// Permite a un usuario leer sus propios datos o a un administrador leer cualquier usuario.
/// </remarks>

public sealed class ReadUserUseCase(
    IReaderAsync<ulong, UserDomain> reader,
    IBusinessValidationService<ulong> validator
) : ReadUseCase<ulong, UserDomain>(reader, validator)
{
    /// <summary>
    /// Ejecuta la lógica para leer la información del usuario actual.
    /// </summary>
    /// <param name="request">Datos de la acción y el ejecutor.</param>
    /// <returns>Resultado con la información del usuario o error de caso de uso.</returns>
    protected override Result<Unit, UseCaseError> ExtraValidation(UserActionDTO<ulong> value)
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN or UserType.PROFESSOR => true,
            _ => value.Data == value.Executor.Id,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
