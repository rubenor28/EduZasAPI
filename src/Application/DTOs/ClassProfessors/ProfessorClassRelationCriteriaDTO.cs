using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.ClassProfessors;

/// <summary>
/// Representa los criterios de búsqueda y filtrado para consultas de relaciones entre profesores y clases.
/// </summary>
public sealed record ProfessorClassRelationCriteriaDTO : CriteriaDTO
{
    /// <summary>
    /// Obtiene o establece el filtro opcional para el identificador del profesor en la relación.
    /// </summary>
    public Optional<ulong> UserId { get; set; } = Optional<ulong>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para el identificador de la clase en la relación.
    /// </summary>
    public Optional<string> ClassId { get; set; } = Optional<string>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para indicar si el profesor es propietario de la clase.
    /// </summary>
    public Optional<bool> IsOwner { get; set; } = Optional<bool>.None();
}
