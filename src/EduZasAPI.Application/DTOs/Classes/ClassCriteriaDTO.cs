using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

/// <summary>
/// Representa los criterios de búsqueda y filtrado para consultas de clases.
/// </summary>
public class ClassCriteriaDTO : ICriteriaDTO
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
    public Optional<ulong> WithProfessor { get; set; } = Optional<ulong>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para clases que tienen inscrito un estudiante específico.
    /// </summary>
    public Optional<ulong> WithStudent { get; set; } = Optional<ulong>.None();
}
