using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Contacts;

public class ContactEFReader(
    EduZasDotnetContext ctx,
    IMapper<AgendaContact, ContactDomain> domainMapper
) : CompositeKeyEFReader<ContactIdDTO, ContactDomain, AgendaContact>(ctx, domainMapper)
{
    public override async Task<AgendaContact?> GetTrackedById(ContactIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(c => c.ContactId == id.ContactId)
            .Where(c => c.AgendaOwnerId == id.AgendaOwnerId)
            .FirstOrDefaultAsync();
}
