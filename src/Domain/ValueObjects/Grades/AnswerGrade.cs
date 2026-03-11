using Domain.ValueObjects.Grades;
using System.Collections.Generic;

/// <summary>
/// Representa la calificación general de una respuesta de un estudiante, incluyendo el desglose por pregunta.
/// </summary>
public record AnswerGrade
{
    /// <summary>
    /// ID del estudiante.
    /// </summary>
    public required ulong StudentId { get; init; }
    /// <summary>
    /// Puntos totales obtenidos.
    /// </summary>
    public required uint Points { get; init; }
    /// <summary>
    /// Puntos totales posibles.
    /// </summary>
    public required uint TotalPoints { get; init; }
    /// <summary>
    /// Desglose de la calificación para cada pregunta.
    /// </summary>
    public required IEnumerable<Grade> GradeDetails { get; init; }
}
