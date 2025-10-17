using Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.Classes;

public record class WithProfessorMAPI
{
    public required ulong Id { get; set; }
    public bool? IsOwner { get; set; } = null;
}

public record class WithStudentMAPI
{
    public required ulong Id { get; set; }
    public bool? Hidden { get; set; } = null;
}

/// <summary>
/// Representa los criterios de búsqueda y filtrado para consultas de clases
/// de Minimal API.
/// </summary>
public sealed record ClassCriteriaMAPI : CriteriaDTO
{
    /// <summary>
    /// Obtiene o establece el filtro opcional para el estado de activación de la clase.
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para el nombre de la clase.
    /// </summary>
    public StringQueryMAPI? ClassName { get; set; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para la materia o asignatura de la clase.
    /// </summary>
    public StringQueryMAPI? Subject { get; set; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para la sección o grupo de la clase.
    /// </summary>
    public StringQueryMAPI? Section { get; set; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para clases que tienen asignado un profesor específico.
    /// </summary>
    public WithProfessorMAPI? WithProfessor { get; set; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para clases que tienen inscrito un estudiante específico.
    /// </summary>
    public WithStudentMAPI? WithStudent { get; set; }
}
