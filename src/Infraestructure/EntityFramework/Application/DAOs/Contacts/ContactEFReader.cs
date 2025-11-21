using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Contacts;

public class ContactEFReader(
    EduZasDotnetContext ctx,
    IMapper<AgendaContact, ContactDomain> domainMapper
) : EFReader<ContactIdDTO, ContactDomain, AgendaContact>(ctx, domainMapper)
{
    public override async Task<AgendaContact?> GetTrackedById(ContactIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(c => c.UserId == id.UserId)
            .Where(c => c.AgendaOwnerId == id.AgendaOwnerId)
            .FirstOrDefaultAsync();
}
