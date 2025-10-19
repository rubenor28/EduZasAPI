using Application.DTOs.Users;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

public class UserEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<User, UserDomain> domainMapper,
    IUpdateMapper<UserUpdateDTO, User> updateMapper
) : SimpleKeyEFUpdater<ulong, UserDomain, UserUpdateDTO, User>(ctx, domainMapper, updateMapper);
