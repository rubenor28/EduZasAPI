using Application.DTOs.Users;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Users;

/// <summary>
/// Implementación de actualización de usuarios usando EF.
/// </summary>
public class UserEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<User, UserDomain> domainMapper,
    IUpdateMapper<UserUpdateDTO, User> updateMapper
) : EFUpdater<UserDomain, UserUpdateDTO, User>(ctx, domainMapper, updateMapper)
{
    /// <inheritdoc/>
    protected override Task<User?> GetTrackedByDTO(UserUpdateDTO value) =>
        _dbSet.AsTracking().AsQueryable().Where(u => u.UserId == value.Id).FirstOrDefaultAsync();
}
