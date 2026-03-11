using Domain.ValueObjects.Grades;
using System.Collections.Generic;

namespace Domain.ValueObjects.Reports;

/// <summary>
/// Representa el resultado de un estudiante en un examen.
/// </summary>
public record StudentResult
{
    /// <summary>
    /// ID del estudiante.
    /// </summary>
    public required ulong StudentId { get; init; }
    /// <summary>
    /// Calificación obtenida.
    /// </summary>
    public required double Grade { get; init; }
}

/// <summary>
/// Extiende <see cref="StudentResult"/> para incluir el nombre del estudiante.
/// </summary>
public record StudentResultDetail : StudentResult
{
    /// <summary>
    /// Nombre completo del estudiante.
    /// </summary>
    public required string StudentName { get; init; }
}

/// <summary>
/// Reporte de estadísticas y resultados de un examen para una clase.
/// </summary>
public record ClassTestReport
{
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
    /// Umbral de aprobación.
    /// </summary>
    public required double PassThreshold { get; init; }
    /// <summary>
    /// Fecha del examen.
    /// </summary>
    public required DateTimeOffset TestDate { get; init; }

    /// <summary>
    /// Porcentaje promedio de la clase.
    /// </summary>
    public required double AveragePercentage { get; init; }
    /// <summary>
    /// Mediana de los porcentajes de la clase.
    /// </summary>
    public required double MedianPercentage { get; init; }
    /// <summary>
    * Porcentaje de estudiantes que aprobaron.
    /// </summary>
    public required double PassPercentage { get; init; }
    /// <summary>
    /// Desviación estándar de las calificaciones.
    /// </summary>
    public required double StandardDeviation { get; init; }
    /// <summary>
    /// Puntuación máxima obtenida.
    /// </summary>
    public required double MaxScore { get; init; }
    /// <summary>
    /// Puntuación mínima obtenida.
    /// </summary>
    public required double MinScore { get; init; }
    /// <summary>
    /// Número total de estudiantes.
    /// </summary>
    public required int TotalStudents { get; init; }

    /// <summary>
    /// Lista de resultados detallados de los estudiantes.
    /// </summary>
    public required IEnumerable<StudentResultDetail> Results { get; init; }
    /// <summary>
    /// Lista de errores de calificación individual.
    /// </summary>
    public required IEnumerable<IndividualGradeErrorDetail> Errors { get; init; }
}
