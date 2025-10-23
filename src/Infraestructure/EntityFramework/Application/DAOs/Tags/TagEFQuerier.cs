using Application.DTOs.Tags;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tags;

public class TagEFQuerier(
    EduZasDotnetContext ctx,
    IMapper<Tag, TagDomain> domainMapper,
    int pageSize
) : EFQuerier<TagDomain, TagCriteriaDTO, Tag>(ctx, domainMapper, pageSize)
{
    public override IQueryable<Tag> BuildQuery(TagCriteriaDTO criteria) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereStringQuery(criteria.Text, t => t.Text)
            .WhereOptional(
                criteria.OwnerAgendaId,
                ownerId => tag => tag.TagsPerUsers.Any(tpu => tpu.Contact.AgendaOwnerId == ownerId)
            );
}
