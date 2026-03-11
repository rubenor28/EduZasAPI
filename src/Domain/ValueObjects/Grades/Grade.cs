using System.Text.Json.Serialization;
using Domain.Entities.Questions;

namespace Domain.ValueObjects.Grades;

/// <summary>
/// Clase base abstracta para la calificación de una pregunta. Define la estructura común para los diferentes tipos de calificación.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ConceptRelationGrade), typeDiscriminator: QuestionTypes.ConceptRelation)]
[JsonDerivedType(typeof(MultipleSelectionGrade), typeDiscriminator: QuestionTypes.MultipleSelection)]
[JsonDerivedType(typeof(MultipleChoiseGrade), typeDiscriminator: QuestionTypes.MultipleChoise)]
[JsonDerivedType(typeof(OrderingGrade), typeDiscriminator: QuestionTypes.Ordering)]
[JsonDerivedType(typeof(OpenGrade), typeDiscriminator: QuestionTypes.Open)]
public abstract record Grade
{
    /// <summary>
    /// Título de la pregunta.
    /// </summary>
    public required string Title { get; init; }
    /// <summary>
    /// ID de la pregunta.
    /// </summary>
    public required Guid QuestionId { get; init; }
    /// <summary>
    /// Puntos totales posibles para la pregunta.
    /// </summary>
    public abstract uint TotalPoints { get; }
    /// <summary>
    /// Indica si la pregunta ha sido calificada manualmente. Nulo si no aplica.
    /// </summary>
    public bool? ManualGrade { get; init; } = null;
    /// <summary>
    /// Puntos obtenidos. Si hay calificación manual, usa los puntos totales; de lo contrario, calcula los aciertos.
    /// </summary>
    public uint Points => ManualGrade == true ? TotalPoints : Asserts();

    /// <summary>
    /// Método abstracto para calcular el número de aciertos.
    /// </summary>
    /// <returns>Número de aciertos.</returns>
    public abstract uint Asserts();
}
