using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Contacts;

public sealed record ContactCriteriaDTO : CriteriaDTO
{
    /// <summary>
    /// Alias establecido para el contacto
    /// </summary>
    public Optional<StringQueryDTO> Alias { get; set; } = Optional<StringQueryDTO>.None();

    /// <summary>
    /// Id del usuario que agrega el contacto
    /// </summary>
    public Optional<ulong> AgendaOwnerId { get; set; } = Optional<ulong>.None();

    /// <summary>
    /// Id de nuestro usuario que queremos agregar como contacto
    /// </summary>
    public Optional<ulong> ContactId { get; set; } = Optional<ulong>.None();

    public Optional<IEnumerable<string>> Tags {get;set;} = Optional<IEnumerable<string>>.None();
}
