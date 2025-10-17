using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

public class UserEFDeleter : SimpleKeyEFDeleter<ulong, UserDomain, User>
{
    public UserEFDeleter(EduZasDotnetContext ctx, IMapper<User, UserDomain> domainMapper)
        : base(ctx, domainMapper) { }
}
