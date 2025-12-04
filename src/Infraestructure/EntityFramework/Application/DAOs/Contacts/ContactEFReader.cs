using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Contacts;

public class ContactEFReader(EduZasDotnetContext ctx, IMapper<AgendaContact, ContactDomain> mapper)
    : EFReader<ContactIdDTO, ContactDomain, AgendaContact>(ctx, mapper)
{
    protected override Expression<Func<AgendaContact, bool>> GetIdPredicate(ContactIdDTO id) =>
        c => c.UserId == id.UserId && c.AgendaOwnerId == id.AgendaOwnerId;
}
