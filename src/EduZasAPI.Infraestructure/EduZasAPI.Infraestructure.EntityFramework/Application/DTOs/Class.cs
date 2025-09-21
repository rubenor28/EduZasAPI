namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class ClassEF
{
    public string ClassId { get; set; } = null!;

    public bool? Active { get; set; }

    public string ClassName { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Section { get; set; } = null!;

    public ulong OwnerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual ICollection<ClassProfessorEF> ClassProfessors { get; set; } = new List<ClassProfessorEF>();

    public virtual ICollection<ClassStudentEF> ClassStudents { get; set; } = new List<ClassStudentEF>();

    public virtual ICollection<NotificationEF> Notifications { get; set; } = new List<NotificationEF>();

    public virtual UserEF Owner { get; set; } = null!;

    public virtual ICollection<TestsPerClassEF> TestsPerClasses { get; set; } = new List<TestsPerClassEF>();

    public virtual ICollection<UserEF> Professors { get; set; } = new List<UserEF>();
}
