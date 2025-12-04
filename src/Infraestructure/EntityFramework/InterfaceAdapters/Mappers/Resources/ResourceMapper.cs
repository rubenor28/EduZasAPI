using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

public sealed class ResourceMapper : IMapper<Resource, ResourceDomain>
{
    public ResourceDomain Map(Resource input)
      => new() {
  Id = input.ResourceId,
  Active = input.Active ?? false,
  Content = input.Content,
  Title = input.Title,
  CreatedAt = input.CreatedAt,
  ModifiedAt = input.ModifiedAt,
  ProfessorId = input.ProfessorId
      };
}
