using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

public class UserEFReader(EduZasDotnetContext ctx, IEFProjector<User, UserDomain> projector)
    : EFReader<ulong, UserDomain, User>(ctx, projector)
{
    protected override Expression<Func<User, bool>> GetIdPredicate(ulong id) => u => u.UserId == id;
}
