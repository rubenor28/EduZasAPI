using Application.DTOs.Contacts;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Contact;

public sealed class ContactEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<AgendaContact, ContactDomain> domainMapper,
    IUpdateMapper<ContactUpdateDTO, AgendaContact> updateMapper
)
    : SimpleKeyEFUpdater<ulong, ContactDomain, ContactUpdateDTO, AgendaContact>(
        ctx,
        domainMapper,
        updateMapper
    );
