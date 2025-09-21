namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class ClassProfessorEF
{
    public string ClassId { get; set; } = null!;

    public ulong ProfessorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ClassEF Class { get; set; } = null!;

    public virtual UserEF Professor { get; set; } = null!;
}
