namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class ClassStudentEF
{
    public string ClassId { get; set; } = null!;

    public ulong StudentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ClassEF Class { get; set; } = null!;

    public virtual UserEF Student { get; set; } = null!;
}
