using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Contacts;

/// <summary>
/// Implementación de eliminación de contactos usando EF.
/// </summary>
public sealed class ContactEFDeleter(
    EduZasDotnetContext ctx,
    IMapper<AgendaContact, ContactDomain> domainMapper
) : EFDeleter<ContactIdDTO, ContactDomain, AgendaContact>(ctx, domainMapper)
{
    /// <inheritdoc/>
    public override async Task<AgendaContact?> GetTrackedById(ContactIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(c => c.AgendaOwnerId == id.AgendaOwnerId)
            .Where(c => c.UserId == id.UserId)
            .FirstOrDefaultAsync();
}
