using Application.DAOs;
using Application.DTOs.ClassTests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassTests;

/// <summary>
/// Implementación de actualización de relaciones Clase-Examen usando EF.
/// </summary>
public sealed class ClassTestEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<TestPerClass, ClassTestDomain> domainMapper,
    IUpdateMapper<ClassTestDTO, TestPerClass> updateMapper
) : EFUpdater<ClassTestDomain, ClassTestDTO, TestPerClass>(ctx, domainMapper, updateMapper)
{
    /// <inheritdoc/>
    protected override Task<TestPerClass?> GetTrackedByDTO(ClassTestDTO value) =>
        _dbSet.AsTracking().FirstOrDefaultAsync(tpc => tpc.TestId == value.TestId && tpc.ClassId == value.ClassId);
}