namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de relación Examen-Clase.
/// </summary>
public partial class TestPerClass
{
    public Guid TestId { get; set; }

    public string ClassId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
