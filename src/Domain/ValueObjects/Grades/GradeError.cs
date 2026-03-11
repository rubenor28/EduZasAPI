namespace Domain.ValueObjects.Grades;

/// <summary>
/// Representa un error abstracto relacionado con la calificación.
/// </summary>
public abstract record GradeError;

/// <summary>
/// Fábrica para crear tipos específicos de errores de calificación.
/// </summary>
public static class GradeErrors
{
    /// <summary>
    /// Crea un error que indica que una o más preguntas requieren calificación manual.
    /// </summary>
    /// <param name="questionsId">Enumeración de los IDs de las preguntas que necesitan calificación.</param>
    /// <returns>Una instancia de <see cref="MissingManualGrade"/>.</returns>
    public static GradeError MissingManualGrade(IEnumerable<Guid> questionsId) =>
        new MissingManualGrade(questionsId);
}

/// <summary>
/// Error específico que indica que una o más preguntas requieren calificación manual.
/// </summary>
/// <param name="QuestionId">Enumeración de los IDs de las preguntas que necesitan calificación.</param>
public sealed record MissingManualGrade(IEnumerable<Guid> QuestionId) : GradeError;
