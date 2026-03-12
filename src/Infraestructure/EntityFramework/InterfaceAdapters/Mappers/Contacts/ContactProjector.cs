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
    /// <summary>
    /// Obtiene la expresión de proyección para convertir una entidad de contacto a un objeto de dominio.
    /// </summary>
    /// <param name="criteria">Criterios de consulta.</param>
    /// <returns>Expresión de proyección.</returns>
    public Expression<Func<AgendaContact, ContactDomain>> GetProjection(ContactCriteriaDTO criteria) =>
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
