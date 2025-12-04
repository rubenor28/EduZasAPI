using Application.DTOs.Common;

namespace Application.DTOs.Contacts;

public sealed record ContactCriteriaDTO : CriteriaDTO
{
    ///<summary>
    /// Alias del contacto. Campo opcional.
    ///</summary>
    public StringQueryDTO? Alias { get; init; }

    ///<summary>
    /// ID usuario del dueño de la agenda. Campo opcional.
    ///</summary>
    public ulong? AgendaOwnerId { get; init; }

    ///<summary>
    /// ID usuario del dueño del contacto registrado. Campo opcional.
    ///</summary>
    public ulong? UserId { get; init; }

    ///<summary>
    /// Etiquetas del usuario. Campo opcional.
    ///</summary>
    public IEnumerable<string>? Tags { get; init; }
}
