using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.UserNotifications;

public class UserNotificationEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<NotificationPerUser, UserNotificationDomain> domainMapper,
    IUpdateMapper<UserNotificationDomain, NotificationPerUser> updateMapper
)
    : CompositeKeyEFUpdater<UserNotificationIdDTO, UserNotificationDomain, NotificationPerUser>(
        ctx,
        domainMapper,
        updateMapper
    )
{
    protected override async Task<NotificationPerUser?> GetTrackedById(UserNotificationIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(n => n.NotificationId == id.NotificationId)
            .Where(n => n.UserId == id.UserId)
            .FirstOrDefaultAsync();
}
