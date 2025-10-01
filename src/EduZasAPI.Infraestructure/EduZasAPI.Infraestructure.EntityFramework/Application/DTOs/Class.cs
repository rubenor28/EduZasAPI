using EduZasAPI.Infraestructure.EntityFramework.Application.Notifications;
using EduZasAPI.Infraestructure.EntityFramework.Application.ClassStudents;
using EduZasAPI.Infraestructure.EntityFramework.Application.ClassProfessors;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;
using EduZasAPI.Infraestructure.EntityFramework.Application.TestsPerClass;

using EduZasAPI.Infraestructure.EntityFramework.Domain.Common;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Classes;

public partial class Class : ISoftDeletableEF
{
    public string ClassId { get; set; } = null!;

    public bool? Active { get; set; }

    public string ClassName { get; set; } = null!;

    public string? Subject { get; set; } = null!;

    public string? Section { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual ICollection<ClassProfessor> ClassProfessors { get; set; } = new List<ClassProfessor>();

    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<TestPerClass> TestsPerClasses { get; set; } = new List<TestPerClass>();

    public virtual ICollection<User> Professors { get; set; } = new List<User>();
}
