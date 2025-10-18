using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

public class NotificationEFReader : SimpleKeyEFReader<ulong, UserDomain, User>
{
    public NotificationEFReader(EduZasDotnetContext ctx, IMapper<User, UserDomain> domainMapper)
        : base(ctx, domainMapper) { }
}
