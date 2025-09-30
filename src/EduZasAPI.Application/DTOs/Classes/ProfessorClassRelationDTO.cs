namespace EduZasAPI.Application.Classes;

/// <summary>
/// Representa los datos para asociar profesores a clases.
/// </summary>
public class ProfessorClassRelationDTO
{
    public required ClassUserRelationIdDTO Id { get; set; }
}
