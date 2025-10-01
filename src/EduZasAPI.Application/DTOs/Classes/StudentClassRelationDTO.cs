using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Classes;

public class StudentClassRelationDTO : IIdentifiable<ClassUserRelationIdDTO>
{
    public required ClassUserRelationIdDTO Id { get; set; }
}
