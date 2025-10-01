using EduZasAPI.Infraestructure.EntityFramework.Application.Classes;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.ClassProfessors;

public partial class ClassProfessor
{
    public string ClassId { get; set; } = null!;

    public ulong ProfessorId { get; set; }

    public bool? IsOwner {get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User Professor { get; set; } = null!;
}
