using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Classes;

/// <summary>
/// Implementación de eliminación de clases usando EF.
/// </summary>
public class ClassEFDeleter(EduZasDotnetContext ctx, IMapper<Class, ClassDomain> domainMapper)
    : EFDeleter<string, ClassDomain, Class>(ctx, domainMapper)
{
    /// <inheritdoc/>
    public override async Task<Class?> GetTrackedById(string id) =>
        await _dbSet.AsTracking().AsQueryable().Where(c => c.ClassId == id).FirstOrDefaultAsync();
}
