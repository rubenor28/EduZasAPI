using Application.DTOs.Users;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

/// <summary>
/// Implementación de creación de usuarios usando EF.
/// </summary>
public class UserEFCreator(
    EduZasDotnetContext ctx,
    IMapper<User, UserDomain> domainMapper,
    IMapper<NewUserDTO, User> newEntityMapper
) : EFCreator<UserDomain, NewUserDTO, User>(ctx, domainMapper, newEntityMapper);
