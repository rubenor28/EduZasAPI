namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de respuesta de examen.
/// </summary>
public partial class Answer
{
    public string Content { get; set; } = null!;
    public ulong UserId { get; set; }
    public Guid TestId { get; set; }
    public string ClassId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual TestPerClass TestPerClass { get; set; } = null!;
}
