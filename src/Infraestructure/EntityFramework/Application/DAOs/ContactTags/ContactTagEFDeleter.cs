using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ContactTags;

/// <summary>
/// Implementación de eliminación de etiquetas de contacto usando EF.
/// </summary>
public sealed class ContactTagEFDeleter(
    EduZasDotnetContext ctx,
    IMapper<ContactTag, ContactTagDomain> domainMapper
) : EFDeleter<ContactTagIdDTO, ContactTagDomain, ContactTag>(ctx, domainMapper)
{
    /// <summary>
    /// Obtiene la entidad de etiqueta de contacto rastreada por su ID compuesto para eliminación.
    /// </summary>
    /// <param name="id">ID compuesto.</param>
    /// <returns>Entidad rastreada o null.</returns>
    public override Task<ContactTag?> GetTrackedById(ContactTagIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(tpu => tpu.TagId == id.TagId)
            .Where(tpu => tpu.UserId == id.UserId)
            .Where(tpu => tpu.AgendaOwnerId == id.AgendaOwnerId)
            .FirstOrDefaultAsync();
}
