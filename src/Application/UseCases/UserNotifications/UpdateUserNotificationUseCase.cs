using Application.DAOs;
using Application.DTOs.UserNotifications;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.UserNotifications;

/// <summary>
/// Caso de uso para actualizar el estado de una notificación de usuario (marcar como leída).
/// </summary>
public sealed class UpdateUserNotificationUseCase(
    IUpdaterAsync<UserNotificationDomain, UserNotificationUpdateDTO> updater,
    IReaderAsync<UserNotificationIdDTO, UserNotificationDomain> reader,
    IBusinessValidationService<UserNotificationUpdateDTO>? validator = null
)
    : UpdateUseCase<UserNotificationIdDTO, UserNotificationUpdateDTO, UserNotificationDomain>(
        updater,
        reader,
        validator
    )
{
    /// <summary>
    /// Obtiene el identificador compuesto de la notificación de usuario desde el DTO.
    /// </summary>
    /// <param name="dto">DTO de actualización.</param>
    /// <returns>Identificador compuesto de la notificación del usuario.</returns>
    protected override UserNotificationIdDTO GetId(UserNotificationUpdateDTO dto) =>
        new() { UserId = dto.UserId, NotificationId = dto.NotificationId };

    /// <summary>
    /// Valida que el ejecutor sea el destinatario de la notificación o un administrador.
    /// </summary>
    /// <param name="value">Datos de la actualización.</param>
    /// <param name="original">Entidad de notificación de usuario original.</param>
    /// <returns>Resultado exitoso o error de autorización.</returns>
    protected override Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<UserNotificationUpdateDTO> value,
        UserNotificationDomain original
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => value.Data.UserId == value.Executor.Id,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
