using Application.DTOs.Contacts;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Contacts;

public sealed class ContactEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<AgendaContact, ContactDomain> domainMapper,
    IUpdateMapper<ContactUpdateDTO, AgendaContact> updateMapper
)
    : CompositeKeyEFUpdater<ContactIdDTO, ContactDomain, ContactUpdateDTO, AgendaContact>(
        ctx,
        domainMapper,
        updateMapper
    )
{
    protected override async Task<AgendaContact?> GetTrackedById(ContactIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(c => c.AgendaOwnerId == id.AgendaOwnerId)
            .Where(c => c.ContactId == id.ContactId)
            .FirstOrDefaultAsync();
}
