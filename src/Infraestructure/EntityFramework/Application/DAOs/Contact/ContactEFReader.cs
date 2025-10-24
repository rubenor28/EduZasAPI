using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Contact;

public class ContactEFReader(
    EduZasDotnetContext ctx,
    IMapper<AgendaContact, ContactDomain> domainMapper
) : SimpleKeyEFReader<ulong, ContactDomain, AgendaContact>(ctx, domainMapper);
