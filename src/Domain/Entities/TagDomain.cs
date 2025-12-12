namespace Domain.Entities;

/// <summary>
/// Representa una etiqueta (tag) que puede ser asignada a diferentes entidades, como contactos.
/// </summary>
/// <remarks>
/// La propiedad <see cref="Text"/> es el identificador único de la etiqueta.
/// </remarks>
public sealed record TagDomain
{
    /// <summary>
    /// Obtiene el identificador único de la etiqueta, que es su texto.
    /// </summary>
    public required ulong Id { get; set; }

    /// <summary>
    /// Obtiene o establece el texto único de la etiqueta, que actúa como su clave primaria.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación de la etiqueta.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}
