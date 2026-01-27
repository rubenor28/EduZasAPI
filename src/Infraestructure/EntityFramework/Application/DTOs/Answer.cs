using Domain.Entities;
using Domain.Entities.QuestionAnswers;

namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de respuesta de examen.
/// </summary>
public partial class Answer
{
    public ulong UserId { get; set; }
    public Guid TestId { get; set; }
    public string ClassId { get; set; } = null!;
    public IDictionary<Guid, IQuestionAnswer> Content { get; set; } = null!;
    public bool TryFinished { get; set; }
    public AnswerMetadata Metadata { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual TestPerClass TestPerClass { get; set; } = null!;
}
