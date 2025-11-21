using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Users;

public class UserEFReader(EduZasDotnetContext ctx, IMapper<User, UserDomain> domainMapper)
    : EFReader<ulong, UserDomain, User>(ctx, domainMapper)
{
    public override Task<User?> GetTrackedById(ulong id) =>
        _dbSet.AsTracking().AsQueryable().Where(u => u.UserId == id).FirstOrDefaultAsync();
}
