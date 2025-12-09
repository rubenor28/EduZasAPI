using Application.DTOs.ClassTests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassTests;

/// <summary>
/// Implementación de eliminación de relaciones Clase-Examen usando EF.
/// </summary>
public sealed class ClassTestEFDeleter(
    EduZasDotnetContext ctx,
    IMapper<TestPerClass, ClassTestDomain> domainMapper
) : EFDeleter<ClassTestIdDTO, ClassTestDomain, TestPerClass>(ctx, domainMapper)
{
    /// <inheritdoc/>
    public override Task<TestPerClass?> GetTrackedById(ClassTestIdDTO id) =>
        _dbSet.AsTracking().FirstOrDefaultAsync(tpc => tpc.TestId == id.TestId && tpc.ClassId == id.ClassId);
}
