using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Notifications;

public class NotificationProjector : IEFProjector<Notification, NotificationDomain>
{
    public Expression<Func<Notification, NotificationDomain>> Projection =>
        s =>
            new()
            {
                Id = s.NotificationId,
                Title = s.Title,
                Active = s.Active ?? false,
                ClassId = s.ClassId,
                CreatedAt = s.CreatedAt,
            };

    private static readonly Lazy<Func<Notification, NotificationDomain>> _mapFunc = new(() =>
        new NotificationProjector().Projection.Compile()
    );

    public NotificationDomain Map(Notification s) => _mapFunc.Value(s);
}
