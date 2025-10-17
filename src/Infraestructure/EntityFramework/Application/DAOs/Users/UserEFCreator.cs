using Application.DTOs.Users;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

public class UserEFCreator : EFCreator<UserDomain, NewUserDTO, User>
{
    public UserEFCreator(
        EduZasDotnetContext ctx,
        IMapper<User, UserDomain> domainMapper,
        IMapper<NewUserDTO, User> newEntityMapper
    )
        : base(ctx, domainMapper, newEntityMapper) { }
}
