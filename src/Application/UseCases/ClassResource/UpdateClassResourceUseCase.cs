using Application.DAOs;
using Application.DTOs;
using Application.DTOs.ClassResources;
using Application.DTOs.Common;
using Application.DTOs.Notifications;
using Application.DTOs.UserNotifications;
using Application.DTOs.Users;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.ClassResource;

/// <summary>
/// Caso de uso para actualizar la asociación de un recurso con una clase.
/// </summary>
public sealed class UpdateClassResourceUseCase(
    IUpdaterAsync<ClassResourceDomain, ClassResourceDTO> updater,
    IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> reader,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> classReader,
    ICreatorAsync<NotificationDomain, NewNotificationDTO> notificationCreator,
    IQuerierAsync<UserDomain, UserCriteriaDTO> userQuierier,
    IBulkCreatorAsync<UserNotificationDomain, NewUserNotificationDTO> usrNotificationCreator,
    IBusinessValidationService<ClassResourceDTO>? validator = null
)
    : UpdateUseCase<ClassResourceIdDTO, ClassResourceDTO, ClassResourceDomain>(
        updater,
        reader,
        validator
    )
{

    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;
    private readonly IQuerierAsync<UserDomain, UserCriteriaDTO> _userQuierier = userQuierier;
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;
    private readonly ICreatorAsync<NotificationDomain, NewNotificationDTO> _notificationCreator = notificationCreator;
    private readonly IBulkCreatorAsync<UserNotificationDomain, NewUserNotificationDTO> _usrNotificationCreator = usrNotificationCreator;

    /// <inheritdoc/>
    protected override ClassResourceIdDTO GetId(ClassResourceDTO dto) =>
        new() { ClassId = dto.ClassId, ResourceId = dto.ResourceId };


    protected async override Task ExtraTaskAsync(
        UserActionDTO<ClassResourceDTO> newEntity,
        ClassResourceDomain original,
        ClassResourceDomain createdEntity
    ) {
        if (newEntity.Data.Hidden != original.Hidden && newEntity.Data.Hidden)
            return;

        var user = await _userReader.GetAsync(newEntity.Executor.Id);
        var @class = await _classReader.GetAsync(newEntity.Data.ClassId);

        var notification = await _notificationCreator.AddAsync(
            new()
            {
                ClassId = newEntity.Data.ClassId,
                Title =
                    $"{user!.FirstName} {user.FatherLastname} ha agregado un nuevo recurso en {@class!.ClassName}",
            }
        );

        var usersToNotify = new List<Task>();
        var page = 1;
        PaginatedQuery<UserDomain, UserCriteriaDTO> result;

        do
        {
            result = await _userQuierier.GetByAsync(
                new()
                {
                    Page = page,
                    Active = true,
                    EnrolledInClass = newEntity.Data.ClassId,
                }
            );

            var newNotifications = result.Results.Select(u => new NewUserNotificationDTO
            {
                UserId = u.Id,
                NotificationId = notification.Id,
            });

            await _usrNotificationCreator.AddRangeAsync(newNotifications);

            page++;
        } while (result.Page <= result.TotalPages);
    }
}
