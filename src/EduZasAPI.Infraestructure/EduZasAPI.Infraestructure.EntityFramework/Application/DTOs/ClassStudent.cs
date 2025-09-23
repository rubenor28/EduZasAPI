using EduZasAPI.Infraestructure.EntityFramework.Application.Classes;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.ClassStudents;

public partial class ClassStudent
{
    public string ClassId { get; set; } = null!;

    public ulong StudentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
