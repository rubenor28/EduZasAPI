using System.Text.Json;
using Application.DAOs;
using Application.DTOs.Notifications;
using Application.DTOs.UserNotifications;
using Domain.Entities;
using Quartz;

public class CreateNotificationJob(
    ILogger<CreateNotificationJob> logger,
    ICreatorAsync<NotificationDomain, NewNotificationDTO> notificationCreator,
    IBulkCreatorAsync<UserNotificationDomain, NewUserNotificationDTO> userNotificationCreator
) : IJob
{
    public async Task Execute(IJobExecutionContext ctx)
    {
        var dataMap = ctx.MergedJobDataMap;
        var title = dataMap.GetString("Title") ?? throw new InvalidOperationException();
        var classId = dataMap.GetString("ClassId") ?? throw new InvalidOperationException();

        string json = dataMap.GetString("UserIdsJson") ?? throw new InvalidOperationException();
        IEnumerable<ulong> userIds =
            JsonSerializer.Deserialize<IEnumerable<ulong>>(json)
            ?? throw new InvalidOperationException();

        logger.LogInformation(
            "Creando notificacion en clase {ClsId} con titulo {Title}",
            classId,
            title
        );

        var notification = await notificationCreator.AddAsync(
            new() { Title = title, ClassId = classId }
        );

        var userNotifications = userIds.Select(id => new NewUserNotificationDTO
        {
            NotificationId = notification.Id,
            UserId = id,
        });

        logger.LogInformation("Asignando notificacion con titulo {Title} a usuarios", title);

        await userNotificationCreator.AddRangeAsync(userNotifications);
    }
}
