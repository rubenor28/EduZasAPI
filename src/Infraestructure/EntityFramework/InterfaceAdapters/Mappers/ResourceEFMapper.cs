using Application.DTOs.Resources;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public sealed class ResourceEFMapper
    : IMapper<Resource, ResourceDomain>,
        IMapper<NewResourceDTO, Resource>
{
    public ResourceDomain Map(Resource input) =>
        new()
        {
            Id = input.ResourceId,
            Active = input.Active ?? true,
            Content = input.Content,
            ProfessorId = input.ProfessorId,
            Title = input.Title,
            CreatedAt = input.CreatedAt,
            ModifiedAt = input.ModifiedAt,
        };

    public Resource Map(NewResourceDTO input) =>
        new()
        {
            Title = input.Title,
            Content = input.Content,
            ProfessorId = input.ProfessorId,
        };
}
