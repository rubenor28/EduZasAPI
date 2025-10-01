namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Classes;

public class ProfessorClassRelationMAPI
{
    public required ulong ProfessorId { get; set; }
    public required string ClassId { get; set; }
    public required bool IsOwner { get; set; }
}
