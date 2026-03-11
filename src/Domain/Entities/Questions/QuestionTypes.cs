namespace Domain.Entities.Questions;

/// <summary>
/// Define constantes para los tipos de preguntas admitidos en el sistema.
/// </summary>
public static class QuestionTypes
{
    /// <summary>
    /// Tipo para preguntas de opción múltiple con una sola respuesta correcta.
    /// </summary>
    public const string MultipleChoise = "multiple-choise";
    /// <summary>
    /// Tipo para preguntas de selección múltiple con varias respuestas correctas.
    /// </summary>
    public const string MultipleSelection = "multiple-selection";
    /// <summary>
    /// Tipo para preguntas que requieren ordenar una lista de elementos.
    /// </summary>
    public const string Ordering = "ordering";
    /// <summary>
    /// Tipo para preguntas de respuesta abierta (texto libre).
    /// </summary>
    public const string Open = "open";
    /// <summary>
    /// Tipo para preguntas de relacionar conceptos entre dos columnas.
    /// </summary>
    public const string ConceptRelation = "concept-relation";
};
