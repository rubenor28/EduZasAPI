using System.Linq.Expressions;
using Domain.Entities;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Contacts;

public class ContactProjector : IEFProjector<AgendaContact, ContactDomain>
{
    public Expression<Func<AgendaContact, ContactDomain>> Projection =>
        input =>
            new()
            {
                Id = new() { AgendaOwnerId = input.AgendaOwnerId, UserId = input.UserId },
                Alias = input.Alias,
                Notes = input.Notes.ToOptional(),
                CreatedAt = input.CreatedAt,
                ModifiedAt = input.ModifiedAt,
            };

    private static readonly Lazy<Func<AgendaContact, ContactDomain>> _mapFunc = new(() =>
        new ContactProjector().Projection.Compile()
    );

    public ContactDomain Map(AgendaContact input) => _mapFunc.Value(input);
}
