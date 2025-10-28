using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tags;

public sealed class TagEFReader(EduZasDotnetContext ctx, IMapper<Tag, TagDomain> domainMapper)
    : SimpleKeyEFReader<ulong, TagDomain, Tag>(ctx, domainMapper);
