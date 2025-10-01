namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Classes;

/// <summary>
/// Representa los datos requeridos para crear una nueva clase a través de Minimal API.
/// </summary>
public class NewClassMAPI
{
    /// <summary>
    /// Obtiene o establece el identificador único de la clase.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Obtiene o establece el nombre de la clase.
    /// </summary>
    public required string ClassName { get; set; }

    /// <summary>
    /// Obtiene o establece el identificador del propietario o profesor de la clase.
    /// </summary>
    public required ulong OwnerId { get; set; }

    /// <summary>
    /// Obtiene o establece la materia o asignatura de la clase (opcional).
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Obtiene o establece la sección o grupo de la clase (opcional).
    /// </summary>
    public string? Section { get; set; }
}
