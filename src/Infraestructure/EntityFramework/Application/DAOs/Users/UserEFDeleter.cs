using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Users;

/// <summary>
/// Implementación de eliminación de usuarios usando EF.
/// </summary>
public class UserEFDeleter(EduZasDotnetContext ctx, IMapper<User, UserDomain> domainMapper)
    : EFDeleter<ulong, UserDomain, User>(ctx, domainMapper)
{
    /// <inheritdoc/>
    public override Task<User?> GetTrackedById(ulong id) =>
        _dbSet.AsTracking().AsQueryable().Where(u => u.UserId == id).FirstOrDefaultAsync();
}
