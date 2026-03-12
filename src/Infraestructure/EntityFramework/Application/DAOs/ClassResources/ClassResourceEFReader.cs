using System.Linq.Expressions;
using Application.DTOs.ClassResources;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassResources;

/// <summary>
/// Implementación de lectura de relaciones Clase-Recurso por ID usando EF.
/// </summary>
public sealed class ClassResourceEFReader(
    EduZasDotnetContext ctx,
    IMapper<ClassResource, ClassResourceDomain> mapper
) : EFReader<ClassResourceIdDTO, ClassResourceDomain, ClassResource>(ctx, mapper)
{
    /// <summary>
    /// Obtiene el predicado para filtrar la asociación recurso-clase por su ID compuesto.
    /// </summary>
    /// <param name="id">ID compuesto de la asociación.</param>
    /// <returns>Expresión de predicado.</returns>
    protected override Expression<Func<ClassResource, bool>> GetIdPredicate(ClassResourceIdDTO id) => cr => cr.ClassId == id.ClassId && cr.ResourceId == id.ResourceId;
}
