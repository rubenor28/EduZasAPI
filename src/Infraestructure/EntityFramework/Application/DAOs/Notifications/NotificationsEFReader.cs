using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Notifications;

public class NotificationEFReader(
    EduZasDotnetContext ctx,
    IMapper<Notification, NotificationDomain> domainMapper
) : SimpleKeyEFReader<ulong, NotificationDomain, Notification>(ctx, domainMapper);
