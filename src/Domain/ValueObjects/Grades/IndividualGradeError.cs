namespace Domain.ValueObjects.Grades;

/// <summary>
/// Representa un error de calificación para un estudiante específico.
/// </summary>
/// <param name="UserId">ID del usuario (estudiante).</param>
/// <param name="Error">Mensaje de error.</param>
public record IndividualGradeError(ulong UserId, string Error);

/// <summary>
/// Proporciona detalles sobre un error de calificación individual, incluyendo el nombre del estudiante.
/// </summary>
public record IndividualGradeErrorDetail
{
    /// <summary>
    /// ID del estudiante.
    /// </summary>
    public required ulong StudentId { get; init; }
    /// <summary>
    /// Nombre del estudiante.
    /// </summary>
    public required string StudentName { get; init; }
    /// <summary>
    /// Mensaje de error.
    /// </summary>
    public required string Error { get; init; }
};
