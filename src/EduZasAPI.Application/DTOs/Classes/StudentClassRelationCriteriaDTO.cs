using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

/// <summary>
/// Representa los criterios de búsqueda y filtrado para consultas de relaciones entre estudiantes y clases.
/// </summary>
public class StudentClassRelationCriteriaDTO : ICriteriaDTO
{
    /// <summary>
    /// Obtiene o establece el filtro opcional para el identificador del estudiante en la relación.
    /// </summary>
    public Optional<ulong> UserId { get; set; } = Optional<ulong>.None();

    /// <summary>
    /// Obtiene o establece el filtro opcional para el identificador de la clase en la relación.
    /// </summary>
    public Optional<string> ClassId { get; set; } = Optional<string>.None();

    public Optional<bool> Hidden { get; set; } = Optional<bool>.None();
}
