using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Notifications;
using Application.DTOs.UserNotifications;
using Application.DTOs.Users;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Notifications;

/// <summary>
/// Caso de uso para crear una notificación para todos los alumnos de una clase.
/// </summary>
public sealed class AddNotificationUseCase(
    ICreatorAsync<NotificationDomain, NewNotificationDTO> notificationCreator,
    IBulkCreatorAsync<UserNotificationDomain, NewUserNotificationDTO> userNotificationCreator,
    IQuerierAsync<UserDomain, UserCriteriaDTO> userQuerier
) : AddUseCase<NewNotificationDTO, NotificationDomain>(notificationCreator)
{
    private readonly IBulkCreatorAsync<
        UserNotificationDomain,
        NewUserNotificationDTO
    > _userNotificatoinCreator = userNotificationCreator;

    private readonly IQuerierAsync<UserDomain, UserCriteriaDTO> _userQuerier = userQuerier;

    /// <inheritdoc/>
    protected override async Task ExtraTaskAsync(
        UserActionDTO<NewNotificationDTO> newE,
        NotificationDomain created
    )
    {
        var studentsSearch = await _userQuerier.GetByAsync(
            new() { EnrolledInClass = newE.Data.ClassId }
        );

        var newNotifications = studentsSearch.Results.Select(u => new NewUserNotificationDTO
        {
            UserId = u.Id,
            NotificationId = created.Id,
        });

        await _userNotificatoinCreator.AddRangeAsync(newNotifications);
    }
}
