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
    /// <summary>
    /// Obtiene la entidad de usuario rastreada a partir del DTO de actualización.
    /// </summary>
    /// <param name="value">DTO de actualización.</param>
    /// <returns>Entidad rastreada o null.</returns>
    protected override Task<User?> GetTrackedByDTO(UserUpdateDTO value) =>
        _dbSet.AsTracking().AsQueryable().Where(u => u.UserId == value.Id).FirstOrDefaultAsync();
}
