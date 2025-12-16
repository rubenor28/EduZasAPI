using Domain.Entities.Questions;

namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de examen.
/// </summary>
public partial class Test
{
    public Guid TestId { get; set; }

    public bool Active { get; set; }

    public string Title { get; set; } = null!;

    public string Color { get; set; } = null!;

    public IDictionary<Guid, IQuestion> Content { get; set; } = null!;

    public uint? TimeLimitMinutes { get; set; }

    public ulong ProfessorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual User Professor { get; set; } = null!;

    public virtual ICollection<TestPerClass> TestsPerClasses { get; set; } = new List<TestPerClass>();
}
