using Application.DTOs.Contacts;
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
    /// <summary>
    /// Obtiene la entidad de contacto rastreada por su ID compuesto para eliminación.
    /// </summary>
    /// <param name="id">ID compuesto del contacto.</param>
    /// <returns>Entidad rastreada o null.</returns>
    public async override Task<AgendaContact?> GetTrackedById(ContactIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(c => c.AgendaOwnerId == id.AgendaOwnerId)
            .Where(c => c.UserId == id.UserId)
            .FirstOrDefaultAsync();
}
