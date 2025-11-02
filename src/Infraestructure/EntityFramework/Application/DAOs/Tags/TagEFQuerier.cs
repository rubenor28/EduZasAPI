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
    public override IQueryable<Tag> BuildQuery(TagCriteriaDTO c) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereStringQuery(c.Text, t => t.Text)
            .WhereOptional(
                c.AgendaOwnerId,
                ownerId => tag => tag.TagsPerUsers.Any(tpu => tpu.Contact.AgendaOwnerId == ownerId)
            )
            .WhereOptional(
                c.ContactId,
                contactId => tag => tag.TagsPerUsers.Any(tpu => tpu.Contact.ContactId == contactId)
            );
}
