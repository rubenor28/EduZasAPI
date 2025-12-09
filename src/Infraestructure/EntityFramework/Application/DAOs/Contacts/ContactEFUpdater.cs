using Application.DTOs.Contacts;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Contacts;

/// <summary>
/// Implementación de actualización de contactos usando EF.
/// </summary>
public sealed class ContactEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<AgendaContact, ContactDomain> domainMapper,
    IUpdateMapper<ContactUpdateDTO, AgendaContact> updateMapper
) : EFUpdater<ContactDomain, ContactUpdateDTO, AgendaContact>(ctx, domainMapper, updateMapper)
{
    /// <inheritdoc/>
    protected override async Task<AgendaContact?> GetTrackedByDTO(ContactUpdateDTO value) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(c => c.AgendaOwnerId == value.AgendaOwnerId)
            .Where(c => c.UserId == value.UserId)
            .FirstOrDefaultAsync();
}
