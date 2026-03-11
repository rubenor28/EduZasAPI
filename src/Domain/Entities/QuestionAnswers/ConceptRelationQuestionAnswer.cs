using Domain.Entities.Questions;

namespace Domain.Entities.QuestionAnswers;

/// <summary>
/// Representa la respuesta de un usuario a una pregunta de relación de conceptos.
/// </summary>
public record ConceptRelationQuestionAnswer : IQuestionAnswer
{
    /// <summary>
    /// Obtiene los pares de conceptos que el usuario ha relacionado.
    /// </summary>
    public required ISet<ConceptPair> AnsweredPairs { get; init; }
}
