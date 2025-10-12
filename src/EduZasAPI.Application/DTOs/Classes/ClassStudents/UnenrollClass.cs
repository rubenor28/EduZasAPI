using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

public record class UnenrollClassDTO : IIdentifiable<ClassUserRelationIdDTO>
{
    public required ClassUserRelationIdDTO Id { get; set; }
    public required Executor Executor { get; set; }
}
