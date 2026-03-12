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
    /// <summary>
    /// Obtiene la entidad de asociación examen-clase rastreada por su ID compuesto para eliminación.
    /// </summary>
    /// <param name="id">ID compuesto de la asociación.</param>
    /// <returns>Entidad rastreada o null.</returns>
    public override Task<TestPerClass?> GetTrackedById(ClassTestIdDTO id) =>
        _dbSet.AsTracking().FirstOrDefaultAsync(tpc => tpc.TestId == id.TestId && tpc.ClassId == id.ClassId);
}
