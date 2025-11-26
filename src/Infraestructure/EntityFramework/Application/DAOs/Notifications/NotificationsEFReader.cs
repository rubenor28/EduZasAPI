using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Notifications;

public class NotificationEFReader(
    EduZasDotnetContext ctx,
    IEFProjector<Notification, NotificationDomain> projector
) : EFReader<ulong, NotificationDomain, Notification>(ctx, projector)
{
    protected override Expression<Func<Notification, bool>> GetIdPredicate(ulong id) =>
        n => n.NotificationId == id;
}
