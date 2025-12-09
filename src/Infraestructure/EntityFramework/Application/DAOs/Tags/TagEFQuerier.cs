using Application.DTOs.Tags;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Tags;

/// <summary>
/// Implementación de consulta de etiquetas usando EF.
/// </summary>
public class TagEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<Tag, TagDomain, TagCriteriaDTO> projector,
    int maxPageSize
) : EFQuerier<TagDomain, TagCriteriaDTO, Tag>(ctx, projector, maxPageSize)
{
    /// <inheritdoc/>
    public override IQueryable<Tag> BuildQuery(TagCriteriaDTO c) =>
        _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereStringQuery(c.Text, t => t.Text)
            .WhereOptional(
                c.AgendaOwnerId,
                agendaOwner => c => c.ContactTags.Any(ct => ct.AgendaOwnerId == agendaOwner)
            )
            .WhereOptional(
                c.ContactId,
                contactId => c => c.ContactTags.Any(ct => ct.UserId == contactId)
            );
}
