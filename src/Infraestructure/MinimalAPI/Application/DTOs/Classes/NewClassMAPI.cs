namespace MinimalAPI.Application.DTOs.Classes;

/// <summary>
/// Representa los datos requeridos para crear una nueva clase a través de Minimal API.
/// </summary>
public sealed record NewClassMAPI
{
    /// <summary>
    /// Obtiene o establece el nombre de la clase.
    /// </summary>
    public required string ClassName { get; set; }

    /// <summary>
    /// Obtiene o establece la materia o asignatura de la clase (opcional).
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Obtiene o establece la sección o grupo de la clase (opcional).
    /// </summary>
    public string? Section { get; set; }

    /// <summary>
    /// Obtiene o establece el color de la clase en formato hexadecimal.
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// Id dueño de la clase.
    /// </summary>
    public required ulong OwnerId { get; set; }
}
