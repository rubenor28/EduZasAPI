using Domain.Entities.Questions;

namespace Application.DTOs.Tests;

/// <summary>
/// Representa los datos requeridos para crear una nueva evaluación.
/// </summary>
public sealed record NewTestDTO
{
    /// <summary>
    /// Título descriptivo de la evaluación.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Color de la carta de la evaluación.
    /// </summary>
    public required string Color { get; init; }

    /// <summary>
    /// Estructura JSON que define el contenido de la evaluación (bloques, preguntas).
    /// </summary>
    public required IDictionary<Guid, IQuestion> Content { get; init; }

    /// <summary>
    /// Límite de tiempo en minutos para completar la evaluación (opcional).
    /// </summary>
    public uint? TimeLimitMinutes { get; init; } 

    /// <summary>
    /// Identificador del profesor creador de la evaluación.
    /// </summary>
    public required ulong ProfessorId { get; init; }
}
