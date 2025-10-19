using Application.DTOs.Notifications;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Notifications;

public class NotificationEFQuerier(
    EduZasDotnetContext ctx,
    IMapper<Notification, NotificationDomain> domainMapper,
    int pageSize
)
    : EFQuerier<NotificationDomain, NotificationCriteriaDTO, Notification>(
        ctx,
        domainMapper,
        pageSize
    )
{
    public override IQueryable<Notification> BuildQuery(NotificationCriteriaDTO cr) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereOptional(cr.ClassId, id => n => n.ClassId == id)
            .WhereOptional(
                cr.UserId,
                id => n => n.NotificationPerUsers.Any(nPUsr => nPUsr.UserId == id)
            );
}
