using Application.DTOs.Notifications;
using Domain.ValueObjects;

namespace Application.Services;

public interface ITaskScheduler
{
    Task CreateNotification(NewNotificationDTO notification, IEnumerable<ulong> userIds);
    Task BulkSendEmail(IEnumerable<EmailMessage> notification);
    Task ScheduleMarkAnswersAsFinished(string classId, Guid testId, DateTimeOffset deadline);
}
