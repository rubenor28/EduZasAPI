namespace Domain.Entities;

/// <summary>
/// Representa un contacto en la agenda de un usuario.
/// </summary>
/// <remarks>
/// Esta entidad almacena la relación entre un propietario de agenda y otro usuario
/// (el contacto), junto con metadatos como un alias y notas.
/// </remarks>
public sealed record ContactDomain
{
    /// <summary>
    /// Obtiene o establece el ID del propietario de la agenda.
    /// </summary>
    public required ulong AgendaOwnerId { get; set; }

    /// <summary>
    /// Obtiene o establece el ID del usuario que es el contacto.
    /// </summary>
    public required ulong UserId { get; set; }

    /// <summary>
    /// Obtiene o establece el alias o apodo para el contacto.
    /// </summary>
    public required string Alias { get; set; }

    /// <summary>
    /// Obtiene o establece notas adicionales sobre el contacto (opcional).
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de creación del contacto.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Obtiene o establece la fecha y hora de la última modificación del contacto.
    /// </summary>
    public required DateTimeOffset ModifiedAt { get; set; }
}
