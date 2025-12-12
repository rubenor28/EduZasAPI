using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tags;

/// <summary>
/// Implementación de lectura de etiquetas por ID usando EF.
/// </summary>
public sealed class TagStringEFReader(EduZasDotnetContext ctx, IMapper<Tag, TagDomain> mapper)
    : EFReader<string, TagDomain, Tag>(ctx, mapper)
{
    /// <inheritdoc/>
    protected override Expression<Func<Tag, bool>> GetIdPredicate(string id) => t => t.Text == id;
}
