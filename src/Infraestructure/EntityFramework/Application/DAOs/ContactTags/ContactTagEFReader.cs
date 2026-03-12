using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ContactTags;

/// <summary>
/// Implementación de lectura de etiquetas de contacto por ID usando EF.
/// </summary>
public sealed class ContactTagEFReader(
    EduZasDotnetContext ctx,
    IMapper<ContactTag, ContactTagDomain> mapper
) : EFReader<ContactTagIdDTO, ContactTagDomain, ContactTag>(ctx, mapper)
{
    /// <summary>
    /// Obtiene el predicado para filtrar etiquetas de contacto por su ID compuesto.
    /// </summary>
    /// <param name="id">ID compuesto.</param>
    /// <returns>Expresión de predicado.</returns>
    protected override Expression<Func<ContactTag, bool>> GetIdPredicate(ContactTagIdDTO id) =>
        tpu =>
            tpu.TagId == id.TagId
            && tpu.AgendaOwnerId == id.AgendaOwnerId
            && tpu.UserId == id.UserId;
}
