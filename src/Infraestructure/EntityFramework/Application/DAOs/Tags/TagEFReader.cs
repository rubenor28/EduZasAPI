using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tags;

public sealed class TagEFReader(EduZasDotnetContext ctx, IEFProjector<Tag, TagDomain> projector)
    : EFReader<string, TagDomain, Tag>(ctx, projector)
{
    protected override Expression<Func<Tag, bool>> GetIdPredicate(string id) => t => t.Text == id;
}
