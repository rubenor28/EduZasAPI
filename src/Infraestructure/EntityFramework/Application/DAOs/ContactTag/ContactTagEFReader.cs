using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ContactTag;

public sealed class ContactTagEFReader(
    EduZasDotnetContext ctx,
    IMapper<TagsPerUser, ContactTagDomain> domainMapper
) : CompositeKeyEFReader<ContactTagIdDTO, ContactTagDomain, TagsPerUser>(ctx, domainMapper)
{
    public override async Task<TagsPerUser?> GetTrackedById(ContactTagIdDTO id) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(tpu => tpu.TagId == id.TagId)
            .Where(tpu => tpu.AgendaContactId == tpu.AgendaContactId)
            .FirstOrDefaultAsync();
}
