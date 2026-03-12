using Application.DAOs;
using Application.DTOs.Users;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Users;

/// <summary>
/// Caso de uso para eliminar usuarios.
/// </summary>
/// <remarks>
/// Reglas: Solo administradores pueden eliminar usuarios. No se puede eliminar al último administrador.
/// </remarks>
public sealed class DeleteUserUseCase(
    IDeleterAsync<ulong, UserDomain> deleter,
    IReaderAsync<ulong, UserDomain> reader,
    IQuerierAsync<UserDomain, UserCriteriaDTO> querier,
    IBusinessValidationService<ulong>? validator = null
) : DeleteUseCase<ulong, UserDomain>(deleter, reader, validator)
{
    private readonly IQuerierAsync<UserDomain, UserCriteriaDTO> _querier = querier;

    /// <summary>
    /// Valida que el ejecutor tenga permisos de administrador para eliminar usuarios.
    /// </summary>
    /// <param name="value">Datos de la acción.</param>
    /// <param name="record">Entidad de usuario a eliminar.</param>
    /// <returns>Resultado exitoso o error de autorización.</returns>
    protected override Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<ulong> value,
        UserDomain record
    ) =>
        value.Executor.Role switch
        {
            UserType.ADMIN => Unit.Value,
            _ => UseCaseErrors.Unauthorized(),
        };

    /// <summary>
    /// Valida asíncronamente que no se elimine al último administrador del sistema.
    /// </summary>
    /// <param name="value">Datos de la acción.</param>
    /// <param name="record">Entidad de usuario a eliminar.</param>
    /// <returns>Resultado exitoso o error de conflicto.</returns>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ulong> value,
        UserDomain record
    )
    {
        if (record.Role == UserType.ADMIN)
        {
            var adminsCount = await _querier.CountAsync(new() { Role = UserType.ADMIN });
            if (adminsCount >= 1)
                return UseCaseErrors.Conflict("Debe haber al menos un administrador");
        }

        return Unit.Value;
    }
}
