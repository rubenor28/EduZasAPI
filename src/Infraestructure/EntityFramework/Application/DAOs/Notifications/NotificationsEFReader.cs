using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Notifications;

public class NotificationEFReader(
    EduZasDotnetContext ctx,
    IMapper<Notification, NotificationDomain> mapper
) : EFReader<ulong, NotificationDomain, Notification>(ctx, mapper)
{
    protected override Expression<Func<Notification, bool>> GetIdPredicate(ulong id) =>
        n => n.NotificationId == id;
}
