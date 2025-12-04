using Application.DTOs.Common;

namespace Application.DTOs.Classes;

/// <summary>
/// Representa los criterios de búsqueda para un profesor en una clase.
/// </summary>
public sealed record WithProfessorDTO
{
    /// <summary>
    /// ID del profesor a buscar.
    /// </summary>
    public required ulong Id { get; init; }

    /// <summary>
    /// Indica si el profesor es el propietario de la clase.
    /// </summary>
    public bool? IsOwner { get; init; }
}

/// <summary>
/// Representa los criterios de búsqueda para un estudiante en una clase.
/// </summary>
public sealed record WithStudentDTO
{
    /// <summary>
    /// ID del estudiante a buscar.
    /// </summary>
    public required ulong Id { get; init; }

    /// <summary>
    /// Indica si la clase está oculta para el estudiante.
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
