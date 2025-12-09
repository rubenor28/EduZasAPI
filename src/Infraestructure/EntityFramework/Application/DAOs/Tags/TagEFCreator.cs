using Application.DTOs.Tags;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Tags;

/// <summary>
/// Implementación de creación de etiquetas usando EF.
/// </summary>
public sealed class TagEFCreator(
    EduZasDotnetContext ctx,
    IMapper<Tag, TagDomain> domainMapper,
    IMapper<NewTagDTO, Tag> newEntityMapper
) : EFCreator<TagDomain, NewTagDTO, Tag>(ctx, domainMapper, newEntityMapper);
