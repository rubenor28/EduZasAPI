namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de clase.
/// </summary>
public partial class Class
{
    public string ClassId { get; set; } = null!;

    public bool? Active { get; set; }

    public string ClassName { get; set; } = null!;

    public string? Color { get; set; } = null!;

    public string? Subject { get; set; } = null!;

    public string? Section { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual ICollection<ClassProfessor> ClassProfessors { get; set; } = new List<ClassProfessor>();

    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<TestPerClass> TestsPerClasses { get; set; } = new List<TestPerClass>();

    public virtual ICollection<ClassResource> ClassResources { get; set; } = new List<ClassResource>();

    public virtual ICollection<ResourceViewSession> ResourceViewSessions { get; set; } = new List<ResourceViewSession>();
}
