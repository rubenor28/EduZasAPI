using Application.DTOs.Notifications;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Notifications;

/// <summary>
/// Implementación de consulta de notificaciones usando EF.
/// </summary>
public class NotificationEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Notification, NotificationDomain, NotificationCriteriaDTO> projector,
    int pageSize
)
    : EFQuerier<NotificationDomain, NotificationCriteriaDTO, Notification>(
        ctx,
        projector,
        pageSize
    )
{
    /// <summary>
    /// Construye la consulta de notificaciones a partir de los criterios.
    /// </summary>
    /// <param name="c">Criterios de consulta.</param>
    /// <returns>IQueryable de notificaciones.</returns>
    public override IQueryable<Notification> BuildQuery(NotificationCriteriaDTO c) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereOptional(cr.ClassId, id => n => n.ClassId == id)
            .WhereOptional(
                cr.UserId,
                id => n => n.NotificationPerUsers.Any(nPUsr => nPUsr.UserId == id)
            );
}
