using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ContactTags;

public sealed class ContactTagEFReader(
    EduZasDotnetContext ctx,
    IEFProjector<ContactTag, ContactTagDomain> projector
) : EFReader<ContactTagIdDTO, ContactTagDomain, ContactTag>(ctx, projector)
{
    protected override Expression<Func<ContactTag, bool>> GetIdPredicate(ContactTagIdDTO id) =>
        tpu =>
            tpu.TagText == id.Tag
            && tpu.AgendaOwnerId == id.AgendaOwnerId
            && tpu.UserId == id.UserId;
}
