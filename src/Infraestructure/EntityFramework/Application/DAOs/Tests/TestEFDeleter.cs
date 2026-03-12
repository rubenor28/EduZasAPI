using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tests;

/// <summary>
/// Implementación de eliminación de exámenes usando EF.
/// </summary>
public sealed class TestEFDeleter(EduZasDotnetContext ctx, IMapper<Test, TestDomain> domainMapper)
    : EFDeleter<Guid, TestDomain, Test>(ctx, domainMapper)
{
    /// <summary>
    /// Obtiene la entidad de examen rastreada por su ID para eliminación.
    /// </summary>
    /// <param name="id">ID del examen.</param>
    /// <returns>Entidad rastreada o null.</returns>
    public override Task<Test?> GetTrackedById(Guid id) =>
        _dbSet.AsTracking().AsQueryable().Where(t => t.TestId == id).FirstOrDefaultAsync();
}
