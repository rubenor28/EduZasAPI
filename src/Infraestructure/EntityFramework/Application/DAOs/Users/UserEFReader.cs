using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

public class UserEFReader(EduZasDotnetContext ctx, IMapper<User, UserDomain> mapper)
    : EFReader<ulong, UserDomain, User>(ctx, mapper)
{
    protected override Expression<Func<User, bool>> GetIdPredicate(ulong id) => u => u.UserId == id;
}
