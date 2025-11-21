using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Notifications;

public class NotificationEFReader(
    EduZasDotnetContext ctx,
    IMapper<Notification, NotificationDomain> domainMapper
) : EFReader<ulong, NotificationDomain, Notification>(ctx, domainMapper)
{
    public override async Task<Notification?> GetTrackedById(ulong id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(n => n.NotificationId == id)
            .FirstOrDefaultAsync();
}
