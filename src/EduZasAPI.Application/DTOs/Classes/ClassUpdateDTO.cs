using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Classes;

/// <summary>
/// Representa los datos para actualizar la información de una clase existente en el sistema.
/// </summary>
public class ClassUpdateDTO
{
    /// <summary>
    /// Obtiene o establece el identificador único de la clase a actualizar.
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// Obtiene o establece el estado de la clase.
    /// </summary>
    public required bool Active { get; set; }
    
    /// <summary>
    /// Obtiene o establece el nombre de la clase.
    /// </summary>
    public required string ClassName { get; set; }
    
    /// <summary>
    /// Obtiene o establece la materia o asignatura de la clase (opcional).
    /// </summary>
    public required Optional<string> Subject { get; set; }
    
    /// <summary>
    /// Obtiene o establece la sección o grupo de la clase (opcional).
    /// </summary>
    public required Optional<string> Section { get; set; }
}
