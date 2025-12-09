using Application.DTOs.Common;

namespace Application.DTOs.Classes;

/// <summary>
/// Criterios de filtrado por profesor.
/// </summary>
public sealed record WithProfessorDTO
{
    /// <summary>
    /// Identificador del profesor.
    /// </summary>
    public required ulong Id { get; init; }

    /// <summary>
    /// Filtrar por si es propietario de la clase (opcional).
    /// </summary>
    public bool? IsOwner { get; init; }
}

/// <summary>
/// Criterios de filtrado por estudiante.
/// </summary>
public sealed record WithStudentDTO
{
    /// <summary>
    /// Identificador del estudiante.
    /// </summary>
    public required ulong Id { get; init; }

    /// <summary>
    /// Filtrar por visibilidad de la clase para el estudiante (opcional).
    /// </summary>
    public bool? Hidden { get; init; }
}

/// <summary>
/// Representa los criterios de búsqueda y filtrado para consultas de clases.
/// </summary>
public sealed record ClassCriteriaDTO : CriteriaDTO
{
    /// <summary>
    /// Obtiene o establece el filtro opcional para el estado de activación de la clase.
    /// </summary>
    public bool? Active { get; init; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para el nombre de la clase.
    /// </summary>
    public StringQueryDTO? ClassName { get; init; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para la materia o asignatura de la clase.
    /// </summary>
    public StringQueryDTO? Subject { get; init; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para la sección o grupo de la clase.
    /// </summary>
    public StringQueryDTO? Section { get; init; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para clases que tienen asignado un profesor específico.
    /// </summary>
    public WithProfessorDTO? WithProfessor { get; init; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para clases que tienen inscrito un estudiante específico.
    /// </summary>
    public WithStudentDTO? WithStudent { get; init; }
}
