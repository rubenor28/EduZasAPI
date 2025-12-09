namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de relación Clase-Profesor.
/// </summary>
public partial class ClassProfessor
{
    public string ClassId { get; set; } = null!;

    public ulong ProfessorId { get; set; }

    public bool? IsOwner {get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User Professor { get; set; } = null!;
}
