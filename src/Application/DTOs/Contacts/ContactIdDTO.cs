namespace Application.DTOs.Contacts;

/// <summary>
/// Representa el identificador compuesto para un contacto en la agenda.
/// </summary>
public sealed record ContactIdDTO
{
    /// <summary>
    /// Obtiene o establece el ID del propietario de la agenda.
    /// </summary>
    public required ulong AgendaOwnerId { get; set; }

    /// <summary>
    /// Obtiene o establece el ID del usuario que es el contacto.
    /// </summary>
    public required ulong UserId { get; set; }
}
