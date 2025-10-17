using Application.DTOs.Classes;
using Domain.ValueObjects;

namespace Application.DTOs.ClassProfessors;

/// <summary>
/// Representa la relación entre un profesor y una clase.
/// </summary>
public sealed record ProfessorClassRelationDTO : IIdentifiable<ClassUserRelationIdDTO>
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
