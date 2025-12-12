using Application.DTOs.Contacts;
using Domain.Entities;
using Domain.Extensions;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Contacts;

/// <summary>
/// Implementación de consulta de contactos usando EF.
/// </summary>
public sealed class ContactEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<AgendaContact, ContactDomain, ContactCriteriaDTO> projector,
    int pageSize
) : EFQuerier<ContactDomain, ContactCriteriaDTO, AgendaContact>(ctx, projector, pageSize)
{
    /// <inheritdoc/>
    public override IQueryable<AgendaContact> BuildQuery(ContactCriteriaDTO criteria)
    {
        var query = _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereStringQuery(criteria.Alias, c => c.Alias)
            .WhereOptional(criteria.UserId, userId => contact => contact.UserId == userId)
            .WhereOptional(
                criteria.NotProfessorInClass,
                classId =>
                    contact => !contact.Contact.ClassProfessors.Any(cp => cp.ClassId == classId)
            )
            .WhereOptional(
                criteria.AgendaOwnerId,
                ownerId => contact => contact.AgendaOwnerId == ownerId
            );

        criteria.Tags.IfSome(tags =>
        {
            var distinctTags = tags.Distinct().ToList();
            if (distinctTags.Count > 0)
            {
                query = query.Where(c =>
                    distinctTags.All(tag => c.ContactTags.Any(tpu => tpu.Tag.Text == tag))
                );
            }
        });

        if (_ctx.Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
        {
            query = query.OrderBy(u => u.Alias);
        }

        return query;
    }
}
