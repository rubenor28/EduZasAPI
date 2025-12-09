using System.Linq.Expressions;
using Application.DTOs.Notifications;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Notifications;

/// <summary>
/// Proyector de consultas para notificaciones.
/// </summary>
public class NotificationProjector
    : IEFProjector<Notification, NotificationDomain, NotificationCriteriaDTO>
{
    /// <inheritdoc/>
    public Expression<Func<Notification, NotificationDomain>> GetProjection(
        NotificationCriteriaDTO criteria
    ) =>
        s =>
            new()
            {
                Id = s.NotificationId,
                Title = s.Title,
                Active = s.Active ?? false,
                ClassId = s.ClassId,
                CreatedAt = s.CreatedAt,
            };
}
