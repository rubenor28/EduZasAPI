namespace Domain.ValueObjects.Grades;

/// <summary>
/// Representa una calificación simplificada, con el puntaje de un estudiante.
/// </summary>
public record SimpleGrade
{
    /// <summary>
    /// ID del estudiante.
    /// </summary>
    public required ulong StudentId { get; init; }
    /// <summary>
    /// Puntos obtenidos.
    /// </summary>
    public required uint Points { get; init; }
    /// <summary>
    /// Puntos totales posibles.
    /// </summary>
    public required uint TotalPoints { get; init; }
}
