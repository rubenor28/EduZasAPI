using Domain.Entities;
using Domain.Entities.QuestionAnswers;

namespace EntityFramework.Application.DTOs;

/// <summary>
/// Entidad de respuesta de examen.
/// </summary>
public partial class Answer
{
    /// <summary>
    /// ID del usuario que responde.
    /// </summary>
    public ulong UserId { get; set; }
    
    /// <summary>
    /// ID del examen.
    /// </summary>
    public Guid TestId { get; set; }
    
    /// <summary>
    /// ID de la clase.
    /// </summary>
    public string ClassId { get; set; } = null!;
    
    /// <summary>
    /// Contenido de la respuesta.
    /// </summary>
    public IDictionary<Guid, IQuestionAnswer> Content { get; set; } = null!;
    
    /// <summary>
    /// Indica si el intento ha finalizado.
    /// </summary>
    public bool TryFinished { get; set; }
    
    /// <summary>
    /// Metadatos de la respuesta.
    /// </summary>
    public AnswerMetadata Metadata { get; set; } = null!;
    
    /// <summary>
    /// Fecha de creación de la respuesta.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Fecha de modificación de la respuesta.
    /// </summary>
    public DateTimeOffset ModifiedAt { get; set; }

    /// <summary>
    /// Usuario que responde.
    /// </summary>
    public virtual User User { get; set; } = null!;
    
    /// <summary>
    /// Examen por clase.
    /// </summary>
    public virtual TestPerClass TestPerClass { get; set; } = null!;
}
