using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.UserNotifications;

public class UserNotificationEFReader(
    EduZasDotnetContext ctx,
    IMapper<NotificationPerUser, UserNotificationDomain> domainMapper
)
    : EFReader<UserNotificationIdDTO, UserNotificationDomain, NotificationPerUser>(
        ctx,
        domainMapper
    )
{
    public override Task<NotificationPerUser?> GetTrackedById(UserNotificationIdDTO id) =>
        _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(n => n.UserId == id.UserId)
            .Where(n => n.NotificationId == id.NotificationId)
            .FirstOrDefaultAsync();
}
