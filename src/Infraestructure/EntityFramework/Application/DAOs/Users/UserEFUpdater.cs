using Application.DTOs.Users;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

public class UserEFUpdater : SimpleKeyEFUpdater<ulong, UserDomain, UserUpdateDTO, User>
{
    public UserEFUpdater(
        EduZasDotnetContext ctx,
        IMapper<User, UserDomain> domainMapper,
        IUpdateMapper<UserUpdateDTO, User> updateMapper
    )
        : base(ctx, domainMapper, updateMapper) { }
}
