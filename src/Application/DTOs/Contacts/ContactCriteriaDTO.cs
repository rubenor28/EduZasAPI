using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Contacts;

public sealed record ContactCriteriaDTO : CriteriaDTO
{

    ///<summary>
    /// Alias del contacto. Campo opcional.
    ///</summary>
    public  Optional<StringQueryDTO> Alias { get; set; } = Optional<StringQueryDTO>.None();

    ///<summary>
    /// ID usuario del dueño de la agenda. Campo opcional.
    ///</summary>
    public  Optional<ulong> AgendaOwnerId { get; set; } = Optional<ulong>.None();

    ///<summary>
    /// ID usuario del dueño del contacto registrado. Campo opcional.
    ///</summary>
    public  Optional<ulong> UserId { get; set; } = Optional<ulong>.None();

    ///<summary>
    /// Etiquetas del usuario. Campo opcional.
    ///</summary>
    public  Optional<IEnumerable<string>> Tags { get; set; } = Optional<IEnumerable<string>>.None();
} 
