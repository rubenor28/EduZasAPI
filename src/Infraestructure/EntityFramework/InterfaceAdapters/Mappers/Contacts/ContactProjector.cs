using System.Linq.Expressions;
using Application.DTOs.Contacts;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Contacts;

public class ContactProjector : IEFProjector<AgendaContact, ContactDomain, ContactCriteriaDTO>
{
    public Expression<Func<AgendaContact, ContactDomain>> GetProjection(ContactCriteriaDTO _) =>
        input =>
            new()
            {
                Id = new() { AgendaOwnerId = input.AgendaOwnerId, UserId = input.UserId },
                Alias = input.Alias,
                Notes = input.Notes,
                CreatedAt = input.CreatedAt,
                ModifiedAt = input.ModifiedAt,
            };
}
