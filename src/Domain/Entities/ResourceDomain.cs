using System.Text.Json.Nodes;

namespace Domain.Entities;

/// <summary>
/// Representa un recurso educativo completo, como una nota de clase o material de estudio.
/// </summary>
public sealed record ResourceDomain
{
    /// <summary>
    /// Obtiene o establece el identificador único del recurso (GUID).
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Obtiene o establece si el recurso está activo y disponible.
    /// </summary>
    public required bool Active { get; set; }

    /// <summary>
    /// Obtiene o establece el título del recurso.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Obtiene o establece el color de la tarjeta del recurso.
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// Obtiene o establece el contenido del recurso en formato JSON.
    /// </summary>
    public required JsonNode Content { get; set; }

    /// <summary>
    /// Obtiene o establece el ID del profesor que creó el recurso.
    /// </summary>
    public required ulong ProfessorId { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación del recurso.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la última modificación del recurso.
    /// </summary>
    public required DateTimeOffset ModifiedAt { get; set; }
}

/// <summary>
/// Representa una vista resumida de un recurso educativo, con solo los metadatos esenciales.
/// </summary>
/// <remarks>
/// Utilizado para listas o vistas donde no se necesita cargar el contenido completo del recurso.
/// </remarks>
public sealed record ResourceSummary
{
    /// <summary>
    /// Obtiene o establece el identificador único del recurso (GUID).
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Obtiene o establece si el recurso está activo.
    /// </summary>
    public required bool Active { get; set; }

    /// <summary>
    /// Obtiene o establece el título del recurso.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Obtiene o establece el color de la tarjeta del recurso.
    /// </summary>
    public required string Color { get; set; }

    /// <summary>
    /// Obtiene o establece el ID del profesor que creó el recurso.
    /// </summary>
    public required ulong ProfessorId { get; set; }
}
