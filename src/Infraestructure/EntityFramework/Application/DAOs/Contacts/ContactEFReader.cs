using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Contacts;

public class ContactEFReader(
    EduZasDotnetContext ctx,
    IEFProjector<AgendaContact, ContactDomain> projector
) : EFReader<ContactIdDTO, ContactDomain, AgendaContact>(ctx, projector)
{
    protected override Expression<Func<AgendaContact, bool>> GetIdPredicate(ContactIdDTO id) =>
        c => c.UserId == id.UserId && c.AgendaOwnerId == id.AgendaOwnerId;
}
