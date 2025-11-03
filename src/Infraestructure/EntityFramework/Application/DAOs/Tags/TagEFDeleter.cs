using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tags;

public sealed class TagEFDeleter(EduZasDotnetContext ctx, IMapper<Tag, TagDomain> domainMapper)
    : SimpleKeyEFDeleter<string, TagDomain, Tag>(ctx, domainMapper);
