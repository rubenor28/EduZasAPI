using Application.DAOs;
using Application.DTOs.ClassStudents;
using Application.DTOs.Notifications;
using Application.DTOs.UserNotifications;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Notifications;

public sealed class AddNotificationUseCase(
    ICreatorAsync<NotificationDomain, NewNotificationDTO> notificationCreator,
    ICreatorAsync<UserNotificationDomain, NewUserNotificationDTO> userNotificationCreator,
    IQuerierAsync<StudentClassRelationDTO, StudentClassRelationCriteriaDTO> classStudentsQuerier
) : AddUseCase<NewNotificationDTO, NotificationDomain>(notificationCreator)
{
    private readonly ICreatorAsync<
        UserNotificationDomain,
        NewUserNotificationDTO
    > _userNotificatoinCreator = userNotificationCreator;

    private readonly IQuerierAsync<
        StudentClassRelationDTO,
        StudentClassRelationCriteriaDTO
    > _classStudentsQuerier = classStudentsQuerier;

    // TODO: Crear una interfaz IBulkAdderAsync para paralelizar la creacion de
    // las relaciones de forma thread-safety
    protected override async Task ExtraTaskAsync(
        NewNotificationDTO newEntity,
        NotificationDomain createdEntity
    )
    {
        var studentsSearch = await _classStudentsQuerier.GetByAsync(
            new() { ClassId = newEntity.ClassId }
        );

        var studentIds = studentsSearch.Results.Select(u => u.Id.UserId);

        foreach (var studentId in studentIds)
        {
            await _userNotificatoinCreator.AddAsync(
                new() { UserId = studentId, NotificationId = createdEntity.Id }
            );
        }
    }
}
