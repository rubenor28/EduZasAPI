using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tags;

/// <summary>
/// Implementación de lectura de etiquetas por ID usando EF.
/// </summary>
public sealed class TagEFReader(EduZasDotnetContext ctx, IMapper<Tag, TagDomain> mapper)
    : EFReader<ulong, TagDomain, Tag>(ctx, mapper)
{
    /// <summary>
    /// Obtiene el predicado para filtrar etiquetas por su ID numérico.
    /// </summary>
    /// <param name="id">ID numérico de la etiqueta.</param>
    /// <returns>Expresión de predicado.</returns>
    protected override Expression<Func<Tag, bool>> GetIdPredicate(ulong id) => t => t.TagId == id;
}
