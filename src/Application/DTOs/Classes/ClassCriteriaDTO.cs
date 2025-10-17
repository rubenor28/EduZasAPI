using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Classes;

/// <summary>
/// Representa los criterios de búsqueda para un profesor en una clase.
/// </summary>
public sealed record WithProfessor
{
    /// <summary>
    /// ID del profesor a buscar.
    /// </summary>
    public required ulong Id { get; set; }

    /// <summary>
    /// Indica si el profesor es el propietario de la clase.
    /// </summary>
    public Optional<bool> IsOwner { get; set; } = Optional<bool>.None();
}

/// <summary>
/// Representa los criterios de búsqueda para un estudiante en una clase.
/// </summary>
public sealed record WithStudent
{
    /// <summary>
    /// ID del estudiante a buscar.
    /// </summary>
    public required ulong Id { get; set; }

    /// <summary>
    /// Indica si la clase está oculta para el estudiante.
    /// </summary>
    public Optional<bool> Hidden { get; set; } = Optional<bool>.None();
}

/// <summary>
/// Representa los criterios de búsqueda y filtrado para consultas de clases.
/// </summary>
public sealed record ClassCriteriaDTO : CriteriaDTO
{
    /// <summary>
    /// Obtiene o establece el filtro opcional para el estado de activación de la clase.
    /// </summary>
    public Optional<bool> Active { get; set; } = Optional<bool>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para el nombre de la clase.
    /// </summary>
    public Optional<StringQueryDTO> ClassName { get; set; } = Optional<StringQueryDTO>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para la materia o asignatura de la clase.
    /// </summary>
    public Optional<StringQueryDTO> Subject { get; set; } = Optional<StringQueryDTO>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para la sección o grupo de la clase.
    /// </summary>
    public Optional<StringQueryDTO> Section { get; set; } = Optional<StringQueryDTO>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para clases que tienen asignado un profesor específico.
    /// </summary>
    public Optional<WithProfessor> WithProfessor { get; set; } = Optional<WithProfessor>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para clases que tienen inscrito un estudiante específico.
    /// </summary>
    public Optional<WithStudent> WithStudent { get; set; } = Optional<WithStudent>.None();
}
