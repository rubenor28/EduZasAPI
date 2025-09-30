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
    public required Optional<bool> Active { get; set; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para el nombre de la clase.
    /// </summary>
    public required Optional<StringQueryDTO> ClassName { get; set; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para la materia o asignatura de la clase.
    /// </summary>
    public required Optional<StringQueryDTO> Subject { get; set; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para la sección o grupo de la clase.
    /// </summary>
    public required Optional<StringQueryDTO> Section { get; set; }

    /// <summary>
    /// Obtiene o establece el filtro opcional para el identificador del propietario de la clase.
    /// </summary>
    public required Optional<ulong> OwnerId { get; set; }
}
