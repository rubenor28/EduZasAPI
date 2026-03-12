using Application.DTOs.Classes;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Classes;

/// <summary>
/// Implementación de actualización de clases usando EF.
/// </summary>
public class ClassEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<Class, ClassDomain> domainMapper,
    IUpdateMapper<ClassUpdateDTO, Class> updateMapper
) : EFUpdater<ClassDomain, ClassUpdateDTO, Class>(ctx, domainMapper, updateMapper)
{
    /// <summary>
    /// Obtiene la entidad de clase rastreada a partir del DTO de actualización.
    /// </summary>
    /// <param name="value">DTO de actualización.</param>
    /// <returns>Entidad rastreada o null.</returns>
    protected override async Task<Class?> GetTrackedByDTO(ClassUpdateDTO value) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(c => c.ClassId == value.Id)
            .FirstOrDefaultAsync();
}
