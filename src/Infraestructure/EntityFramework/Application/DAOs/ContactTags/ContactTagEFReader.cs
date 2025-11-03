using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ContactTags;

public sealed class ContactTagEFReader(
    EduZasDotnetContext ctx,
    IMapper<ContactTag, ContactTagDomain> domainMapper
) : CompositeKeyEFReader<ContactTagIdDTO, ContactTagDomain, ContactTag>(ctx, domainMapper)
{
    public override async Task<ContactTag?> GetTrackedById(ContactTagIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(tpu => tpu.TagText == id.Tag)
            .Where(tpu => tpu.AgendaOwnerId == id.AgendaOwnerId)
            .Where(tpu => tpu.ContactId == id.ContactId)
            .FirstOrDefaultAsync();
}
