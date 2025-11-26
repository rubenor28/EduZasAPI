using Application.DTOs.Resources;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Resources;

public class NewResourceEFMapper : IMapper<NewResourceDTO, Resource>
{
    public Resource Map(NewResourceDTO input) =>
        new()
        {
            ResourceId = input.Id,
            Title = input.Title,
            Content = input.Content,
            ProfessorId = input.ProfessorId,
        };
}
