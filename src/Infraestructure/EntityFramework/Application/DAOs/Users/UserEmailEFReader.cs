using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

public class UserEmailEFReader(EduZasDotnetContext ctx, IEFProjector<User, UserDomain> projector)
    : EFReader<string, UserDomain, User>(ctx, projector)
{
    protected override Expression<Func<User, bool>> GetIdPredicate(string email) =>
        u => u.Email == email;
}
