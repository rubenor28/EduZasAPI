namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Classes;

public class ProfessorClassRelationCriteriaMAPI
{
    public ulong? ProfessorId { get; set; }
    public string? ClassId { get; set; }
    public bool? IsOwner { get; set; }
}
