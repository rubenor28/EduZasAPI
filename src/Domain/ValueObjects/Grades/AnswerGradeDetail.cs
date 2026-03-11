namespace Domain.ValueObjects.Grades;

/// <summary>
/// Extiende <see cref="AnswerGrade"/> para incluir detalles contextuales de la calificación de un examen.
/// </summary>
public record AnswerGradeDetail : AnswerGrade
{
    /// <summary>
    /// ID del examen.
    /// </summary>
    public required Guid TestId { get; init; }
    /// <summary>
    /// Nombre de la clase.
    /// </summary>
    public required string ClassName { get; init; }
    /// <summary>
    /// Título del examen.
    /// </summary>
    public required string TestTitle { get; init; }
    /// <summary>
    /// Nombre del profesor.
    /// </summary>
    public required string ProfessorName { get; init; }
    /// <summary>
    /// Nombre del estudiante.
    /// </summary>
    public required string StudentName { get; init; }
    /// <summary>
    /// Puntuación obtenida en el examen.
    /// </summary>
    public required double Score { get; init; }
    /// <summary>
    /// Indica si el estudiante aprobó el examen.
    /// </summary>
    public required bool Approved { get; init; }
    /// <summary>
    /// Fecha en que se realizó el examen.
    /// </summary>
    public required DateTimeOffset Date { get; init; }
}
