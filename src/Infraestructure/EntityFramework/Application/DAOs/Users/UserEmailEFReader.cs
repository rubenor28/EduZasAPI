using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Users;

public class UserEmailEFReader(EduZasDotnetContext ctx, IMapper<User, UserDomain> domainMapper)
    : EFReader<string, UserDomain, User>(ctx, domainMapper)
{
    public override Task<User?> GetTrackedById(string email) =>
        _dbSet.AsTracking().AsQueryable().Where(u => u.Email == email).FirstOrDefaultAsync();
}
