using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ContactTag;

public sealed class ContactTagEFCreator(
    EduZasDotnetContext ctx,
    IMapper<TagsPerUser, ContactTagDomain> domainMapper,
    IMapper<ContactTagIdDTO, TagsPerUser> newEntityMapper
) : EFCreator<ContactTagDomain, ContactTagIdDTO, TagsPerUser>(ctx, domainMapper, newEntityMapper);
