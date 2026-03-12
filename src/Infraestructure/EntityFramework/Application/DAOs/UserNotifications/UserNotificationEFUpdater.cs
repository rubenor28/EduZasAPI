using Application.DTOs.UserNotifications;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.UserNotifications;

/// <summary>
/// Implementación de actualización de notificaciones de usuario usando EF.
/// </summary>
public class UserNotificationEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<NotificationPerUser, UserNotificationDomain> domainMapper,
    IUpdateMapper<UserNotificationUpdateDTO, NotificationPerUser> updateMapper
)
    : EFUpdater<
        UserNotificationDomain,
        UserNotificationUpdateDTO,
        NotificationPerUser
    >(ctx, domainMapper, updateMapper)
{
    /// <summary>
    /// Obtiene la entidad de notificación de usuario rastreada a partir del DTO.
    /// </summary>
    /// <param name="value">DTO de actualización.</param>
    /// <returns>Entidad rastreada o null.</returns>
    protected override Task<NotificationPerUser?> GetTrackedByDTO(UserNotificationUpdateDTO value) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(n => n.NotificationId == dto.NotificationId)
            .Where(n => n.UserId == dto.UserId)
            .FirstOrDefaultAsync();
}
