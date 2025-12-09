using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Notifications;

/// <summary>
/// Implementación de lectura de notificaciones por ID usando EF.
/// </summary>
public class NotificationEFReader(
    EduZasDotnetContext ctx,
    IMapper<Notification, NotificationDomain> mapper
) : EFReader<ulong, NotificationDomain, Notification>(ctx, mapper)
{
    /// <inheritdoc/>
    protected override Expression<Func<Notification, bool>> GetIdPredicate(ulong id) =>
        n => n.NotificationId == id;
}
