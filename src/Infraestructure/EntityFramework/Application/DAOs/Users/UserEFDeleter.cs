using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

public class UserEFDeleter(EduZasDotnetContext ctx, IMapper<User, UserDomain> domainMapper)
    : SimpleKeyEFDeleter<ulong, UserDomain, User>(ctx, domainMapper);
