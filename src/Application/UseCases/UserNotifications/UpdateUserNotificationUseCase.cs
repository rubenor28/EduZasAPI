using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.UserNotifications;
using Application.Services;
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
    /// <inheritdoc/>
    protected override UserNotificationIdDTO GetId(UserNotificationUpdateDTO dto) =>
        new() { UserId = dto.UserId, NotificationId = dto.NotificationId };

    /// <inheritdoc/>
    protected override Result<Unit, UseCaseError> ExtraValidation(
        DTOs.UserActionDTO<UserNotificationUpdateDTO> value,
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
