using Domain.Entities.Questions;

namespace Application.DTOs.Tests;

/// <summary>
/// Datos para actualizar una evaluación existente.
/// </summary>
public sealed record TestUpdateDTO
{
    /// <summary>
    /// Identificador de la evaluación a actualizar.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Nuevo título de la evaluación.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Nuevo color de la evaluación.
    /// </summary>
    public required string Color { get; init; }

    /// <summary>
    /// Nuevo contenido de la evaluación (estructura JSON).
    /// </summary>
    public required IDictionary<Guid, IQuestion> Content { get; init; }

    /// <summary>
    /// Nuevo límite de tiempo en minutos (opcional).
    /// </summary>
    public uint? TimeLimitMinutes { get; init; }

    /// <summary>
    /// Identificador del profesor (para validación de propiedad).
    /// </summary>
    public required ulong ProfessorId { get; init; }

    /// <summary>
    /// Nuevo estado de activación.
    /// </summary>
    public required bool Active { get; init; }
}
