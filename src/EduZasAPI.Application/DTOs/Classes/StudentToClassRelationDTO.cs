namespace EduZasAPI.Application.Classes;

/// <summary>
/// Representa los datos para asociar usuarios (profesores o estudiantes) a clases.
/// </summary>
public class StudentClassRelationDTO
{
    public required ClassUserRelationIdDTO Id { get; set; }
}
