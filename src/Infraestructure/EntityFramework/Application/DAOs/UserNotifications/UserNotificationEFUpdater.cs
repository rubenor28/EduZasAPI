using Application.DTOs.UserNotifications;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.UserNotifications;

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
    protected override async Task<NotificationPerUser?> GetTrackedByDTO(UserNotificationUpdateDTO dto) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(n => n.NotificationId == dto.NotificationId)
            .Where(n => n.UserId == dto.UserId)
            .FirstOrDefaultAsync();
}
