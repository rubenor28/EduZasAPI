using Application.DAOs;
using Application.DTOs.Notifications;
using Application.DTOs.UserNotifications;
using Application.DTOs.Users;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Notifications;

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

    // TODO: Crear una interfaz IBulkAdderAsync para paralelizar la creacion de
    // las relaciones de forma thread-safety
    protected override async Task ExtraTaskAsync(
        NewNotificationDTO newE,
        NotificationDomain created
    )
    {
        var studentsSearch = await _userQuerier.GetByAsync(
            new() { EnrolledInClass = newE.ClassId }
        );

        var newNotifications = studentsSearch.Results.Select(u => new NewUserNotificationDTO
        {
            UserId = u.Id,
            NotificationId = created.Id,
        });

        await _userNotificatoinCreator.AddRangeAsync(newNotifications);
    }
}
