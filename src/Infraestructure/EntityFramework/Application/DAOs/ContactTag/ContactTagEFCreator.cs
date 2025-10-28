using Application.DTOs.ContactTag;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.ContactTag;

public sealed class ContactTagEFCreator(
    EduZasDotnetContext ctx,
    IMapper<TagsPerUser, ContactTagDomain> domainMapper,
    IMapper<NewContactTagDTO, TagsPerUser> newEntityMapper
) : EFCreator<ContactTagDomain, NewContactTagDTO, TagsPerUser>(ctx, domainMapper, newEntityMapper);
