using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.UserNotifications;

/// <summary>
/// Implementación de lectura de notificaciones de usuario por ID usando EF.
/// </summary>
public class UserNotificationEFReader(
    EduZasDotnetContext ctx,
    IMapper<NotificationPerUser, UserNotificationDomain> mapper
) : EFReader<UserNotificationIdDTO, UserNotificationDomain, NotificationPerUser>(ctx, mapper)
{
    /// <inheritdoc/>
    protected override Expression<Func<NotificationPerUser, bool>> GetIdPredicate(
        UserNotificationIdDTO id
    ) => n => n.UserId == id.UserId && n.NotificationId == id.NotificationId;
}
