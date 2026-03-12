using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Classes;

/// <summary>
/// Implementación de lectura de clases por ID usando EF.
/// </summary>
public class ClassEFReader(EduZasDotnetContext ctx, IMapper<Class, ClassDomain> mapper)
    : EFReader<string, ClassDomain, Class>(ctx, mapper)
{
    /// <summary>
    /// Obtiene el predicado para filtrar clases por su ID.
    /// </summary>
    /// <param name="id">ID de la clase.</param>
    /// <returns>Expresión de predicado.</returns>
    protected override Expression<Func<Class, bool>> GetIdPredicate(string id) => c => c.ClassId == id;
}
