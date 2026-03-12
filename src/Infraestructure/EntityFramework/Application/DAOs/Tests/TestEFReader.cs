using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tests;

/// <summary>
/// Implementación de lectura de exámenes por ID usando EF.
/// </summary>
public sealed class TestEFReader(EduZasDotnetContext ctx, IMapper<Test, TestDomain> mapper)
    : EFReader<Guid, TestDomain, Test>(ctx, mapper)
{
    /// <summary>
    /// Obtiene el predicado para filtrar exámenes por su GUID.
    /// </summary>
    /// <param name="id">GUID del examen.</param>
    /// <returns>Expresión de predicado.</returns>
    protected override Expression<Func<Test, bool>> GetIdPredicate(Guid id) => t => t.TestId == id;
}
