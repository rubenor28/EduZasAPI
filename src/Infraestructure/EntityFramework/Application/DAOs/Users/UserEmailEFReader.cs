using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

public class UserEmailEFReader(EduZasDotnetContext ctx, IMapper<User, UserDomain> mapper)
    : EFReader<string, UserDomain, User>(ctx, mapper)
{
    protected override Expression<Func<User, bool>> GetIdPredicate(string email) =>
        u => u.Email == email;
}
