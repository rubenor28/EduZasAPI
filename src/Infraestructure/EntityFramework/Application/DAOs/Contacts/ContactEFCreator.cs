using Application.DTOs.Contacts;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Contacts;

/// <summary>
/// Implementación de creación de contactos usando EF.
/// </summary>
public sealed class ContactEFCreator(
    EduZasDotnetContext ctx,
    IMapper<AgendaContact, ContactDomain> domainMapper,
    IMapper<NewContactDTO, AgendaContact> newEntityMapper
) : EFCreator<ContactDomain, NewContactDTO, AgendaContact>(ctx, domainMapper, newEntityMapper);
