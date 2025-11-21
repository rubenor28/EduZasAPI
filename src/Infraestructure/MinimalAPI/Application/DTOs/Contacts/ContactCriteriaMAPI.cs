using Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.Contacts;

public sealed record ContactCriteriaMAPI : CriteriaDTO
{
    /// <summary>
    /// Alias establecido para el contacto
    /// </summary>
    public StringQueryMAPI? Alias { get; init; } = null;

    /// <summary>
    /// Id del usuario que agrega el contacto
    /// </summary>
    public ulong? AgendaOwnerId { get; init; } = null;

    /// <summary>
    /// Id de nuestro usuario que queremos agregar como contacto
    /// </summary>
    public ulong? UserId { get; init; } = null;

    public IEnumerable<string>? Tags { get; init; }
}
