namespace Application.DTOs.Tests;

/// <summary>
/// Resumen ligero de una evaluación para listados.
/// </summary>
public sealed record TestSummary
{
    /// <summary>
    /// Identificador único de la evaluación.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Título de la evaluación.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Color de la carta de la evaluación.
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// Indica si la evaluación está activa.
    /// </summary>
    public required bool Active { get; set; }

    /// <summary>
    /// Fecha de la última modificación.
    /// </summary>
    public required DateTime ModifiedAt { get; set; }
}
