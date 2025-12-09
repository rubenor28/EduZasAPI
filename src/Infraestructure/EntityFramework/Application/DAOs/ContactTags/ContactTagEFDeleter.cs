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
    /// <inheritdoc/>
    public override async Task<ContactTag?> GetTrackedById(ContactTagIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(tpu => tpu.TagText == id.Tag)
            .Where(tpu => tpu.UserId == id.UserId)
            .Where(tpu => tpu.AgendaOwnerId == id.AgendaOwnerId)
            .FirstOrDefaultAsync();
}
