using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tags;

public sealed class TagEFReader(EduZasDotnetContext ctx, IMapper<Tag, TagDomain> domainMapper)
    : EFReader<string, TagDomain, Tag>(ctx, domainMapper)
{
    public override Task<Tag?> GetTrackedById(string id) =>
        _dbSet.AsTracking().AsQueryable().Where(t => t.Text == id).FirstOrDefaultAsync();
}
