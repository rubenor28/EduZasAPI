namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de clase.
/// </summary>
public partial class Class
{
    /// <summary>
    /// ID de la clase.
    /// </summary>
    public string ClassId { get; set; } = null!;

    /// <summary>
    /// Indica si la clase está activa.
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// Nombre de la clase.
    /// </summary>
    public string ClassName { get; set; } = null!;

    /// <summary>
    /// Color de la clase.
    /// </summary>
    public string? Color { get; set; } = null!;

    /// <summary>
    /// Asignatura de la clase.
    /// </summary>
    public string? Subject { get; set; } = null!;

    /// <summary>
    /// Sección de la clase.
    /// </summary>
    public string? Section { get; set; } = null!;

    /// <summary>
    /// Fecha de creación de la clase.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Fecha de modificación de la clase.
    /// </summary>
    public DateTimeOffset ModifiedAt { get; set; }

    /// <summary>
    /// Profesores de la clase.
    /// </summary>
    public virtual ICollection<ClassProfessor> ClassProfessors { get; set; } = new List<ClassProfessor>();

    /// <summary>
    /// Estudiantes de la clase.
    /// </summary>
    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

    /// <summary>
    /// Notificaciones de la clase.
    /// </summary>
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    /// <summary>
    /// Exámenes por clase.
    /// </summary>
    public virtual ICollection<TestPerClass> TestsPerClasses { get; set; } = new List<TestPerClass>();

    /// <summary>
    /// Recursos de la clase.
    /// </summary>
    public virtual ICollection<ClassResource> ClassResources { get; set; } = new List<ClassResource>();

    /// <summary>
    /// Sesiones de visualización de recursos.
    /// </summary>
    public virtual ICollection<ResourceViewSession> ResourceViewSessions { get; set; } = new List<ResourceViewSession>();
}
