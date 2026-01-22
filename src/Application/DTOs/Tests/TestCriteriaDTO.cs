using Domain.ValueObjects;

namespace Application.DTOs.Tests;

/// <summary>
/// Criterios de búsqueda y filtrado para evaluaciones.
/// </summary>
public sealed record TestCriteriaDTO : CriteriaDTO
{
    /// <summary>
    /// Filtrar por título de la evaluación.
    /// </summary>
    public StringQueryDTO? Title { get; init; }

    /// <summary>
    /// Filtrar por estado de activación.
    /// </summary>
    public bool? Active { get; init; }

    /// <summary>
    /// Filtrar por límite de tiempo exacto (opcional).
    /// </summary>
    // TODO: Caso busqueda de Optional<uint?> para donde el tiempo del test no importe
    public uint? TimeLimitMinutes { get; init; }

    /// <summary>
    /// Filtrar por ID del profesor creador.
    /// </summary>
    public ulong? ProfessorId { get; init; }

    /// <summary>
    /// Filtrar por evaluaciones asignadas a una clase específica.
    /// </summary>
    public string? AssignedInClass { get; init; }
}
