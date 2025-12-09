using Application.DTOs.ContactTags;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ContactTags;

/// <summary>
/// Implementación de creación de etiquetas de contacto usando EF.
/// </summary>
public sealed class ContactTagEFCreator(
    EduZasDotnetContext ctx,
    IMapper<ContactTag, ContactTagDomain> domainMapper,
    IMapper<NewContactTagDTO, ContactTag> newEntityMapper
) : EFCreator<ContactTagDomain, NewContactTagDTO, ContactTag>(ctx, domainMapper, newEntityMapper);
