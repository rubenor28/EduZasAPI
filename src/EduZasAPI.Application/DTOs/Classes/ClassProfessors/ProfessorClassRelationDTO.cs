using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Classes;

/// <summary>
/// Representa la relación entre un profesor y una clase.
/// </summary>
public class ProfessorClassRelationDTO : IIdentifiable<ClassUserRelationIdDTO>
{
    /// <summary>
    /// Obtiene o establece el identificador compuesto de la relación.
    /// </summary>
    public required ClassUserRelationIdDTO Id { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica si el profesor es propietario de la clase.
    /// </summary>
    public bool IsOwner { get; set; } = false;
}
