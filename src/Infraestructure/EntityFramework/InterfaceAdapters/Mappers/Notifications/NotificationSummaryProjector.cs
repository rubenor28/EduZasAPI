using System.Linq.Expressions;
using Application.DTOs.Notifications;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Notifications;

public sealed class NotificationSummaryProjector
    : IEFProjector<NotificationPerUser, NotificationSummaryDTO, NotificationSummaryCriteriaDTO>
{
    public Expression<Func<NotificationPerUser, NotificationSummaryDTO>> GetProjection(
        NotificationSummaryCriteriaDTO criteria
    ) =>
        ef =>
            new(
                Id: ef.NotificationId,
                Readed: ef.Readed,
                Title: ef.Notification.Title,
                ClassId: ef.Notification.ClassId,
                PublishDate: ef.CreatedAt
            );
}
