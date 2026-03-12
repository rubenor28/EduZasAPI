using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Resources;

/// <summary>
/// Implementación de lectura de recursos por ID usando EF.
/// </summary>
public sealed class ResourceEFReader(
    EduZasDotnetContext ctx,
    IMapper<Resource, ResourceDomain> mapper
) : EFReader<Guid, ResourceDomain, Resource>(ctx, mapper)
{
    /// <summary>
    /// Obtiene el predicado para filtrar recursos por su GUID.
    /// </summary>
    /// <param name="id">GUID del recurso.</param>
    /// <returns>Expresión de predicado.</returns>
    protected override Expression<Func<Resource, bool>> GetIdPredicate(Guid id) => r => r.ResourceId == id;
}
