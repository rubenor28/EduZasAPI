using Application.DTOs.Contacts;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Contacts;

public sealed class ContactEFQuerier(
    EduZasDotnetContext ctx,
    IMapper<AgendaContact, ContactDomain> domainMapper,
    int pageSize
) : EFQuerier<ContactDomain, ContactCriteriaDTO, AgendaContact>(ctx, domainMapper, pageSize)
{
    public override IQueryable<AgendaContact> BuildQuery(ContactCriteriaDTO criteria) =>
        _dbSet.AsNoTracking().AsQueryable().WhereStringQuery(criteria.Alias, c => c.Alias);
}

