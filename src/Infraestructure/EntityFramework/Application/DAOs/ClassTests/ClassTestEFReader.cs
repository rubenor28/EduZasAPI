using System.Linq.Expressions;
using Application.DTOs.ClassTests;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ClassTests;

/// <summary>
/// Implementación de lectura de relaciones Clase-Examen por ID usando EF.
/// </summary>
public sealed class ClassTestEFReader(
    EduZasDotnetContext ctx,
    IMapper<TestPerClass, ClassTestDomain> mapper
) : EFReader<ClassTestIdDTO, ClassTestDomain, TestPerClass>(ctx, mapper)
{
    /// <summary>
    /// Obtiene el predicado para filtrar la asociación examen-clase por su ID compuesto.
    /// </summary>
    /// <param name="id">ID compuesto de la asociación.</param>
    /// <returns>Expresión de predicado.</returns>
    protected override Expression<Func<TestPerClass, bool>> GetIdPredicate(ClassTestIdDTO id) =>
        ct => ct.ClassId == id.ClassId && ct.TestId == id.TestId;
}
