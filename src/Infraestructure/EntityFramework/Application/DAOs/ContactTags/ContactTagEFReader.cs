using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ContactTags;

public sealed class ContactTagEFReader(
    EduZasDotnetContext ctx,
    IMapper<ContactTag, ContactTagDomain> mapper
) : EFReader<ContactTagIdDTO, ContactTagDomain, ContactTag>(ctx, mapper)
{
    protected override Expression<Func<ContactTag, bool>> GetIdPredicate(ContactTagIdDTO id) =>
        tpu =>
            tpu.TagText == id.Tag
            && tpu.AgendaOwnerId == id.AgendaOwnerId
            && tpu.UserId == id.UserId;
}
