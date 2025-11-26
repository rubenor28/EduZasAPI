using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ContactTags;

public class ContactTagProjector : IEFProjector<ContactTag, ContactTagDomain>
{
    public Expression<Func<ContactTag, ContactTagDomain>> Projection =>
        input =>
            new()
            {
                Id = new()
                {
                    Tag = input.TagText,
                    AgendaOwnerId = input.AgendaOwnerId,
                    UserId = input.UserId,
                },
                CreatedAt = input.CreatedAt,
            };

    private static readonly Lazy<Func<ContactTag, ContactTagDomain>> _mapFunc = new(() =>
        new ContactTagProjector().Projection.Compile()
    );

    public ContactTagDomain Map(ContactTag input) => _mapFunc.Value(input);
}
