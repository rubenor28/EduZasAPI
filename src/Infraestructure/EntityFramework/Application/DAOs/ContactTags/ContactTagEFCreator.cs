using Application.DTOs.ContactTags;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ContactTags;

public sealed class ContactTagEFCreator(
    EduZasDotnetContext ctx,
    IMapper<ContactTag, ContactTagDomain> domainMapper,
    IMapper<ContactTagDTO, ContactTag> newEntityMapper
) : EFCreator<ContactTagDomain, ContactTagDTO, ContactTag>(ctx, domainMapper, newEntityMapper);
