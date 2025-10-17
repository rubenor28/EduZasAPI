using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

public class UserEFReader : SimpleKeyEFReader<ulong, UserDomain, User>
{
    public UserEFReader(EduZasDotnetContext ctx, IMapper<User, UserDomain> domainMapper)
        : base(ctx, domainMapper) { }
}
