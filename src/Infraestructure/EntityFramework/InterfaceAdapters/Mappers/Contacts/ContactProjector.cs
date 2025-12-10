using System.Linq.Expressions;
using Application.DTOs.Contacts;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Contacts;

/// <summary>
/// Proyector de consultas para contactos.
/// </summary>
public class ContactProjector : IEFProjector<AgendaContact, ContactDomain, ContactCriteriaDTO>
{
    /// <inheritdoc/>
    public Expression<Func<AgendaContact, ContactDomain>> GetProjection(ContactCriteriaDTO _) =>
        input =>
            new()
            {
                AgendaOwnerId = input.AgendaOwnerId,
                UserId = input.UserId,
                Alias = input.Alias,
                Notes = input.Notes,
                CreatedAt = input.CreatedAt,
                ModifiedAt = input.ModifiedAt,
            };
}
