using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Tags;

public sealed record TagCriteriaDTO : CriteriaDTO
{
    /// <summary>
    // Texto de la etiqueta
    /// </summary>
    public Optional<StringQueryDTO> Text { get; set; } = Optional<StringQueryDTO>.None();

    /// <summary>
    // ID de usuario del dueño de la agenda
    /// </summary>
    public Optional<ulong> AgendaOwnerId { get; set; } = Optional<ulong>.None();

    /// <summary>
    /// ID de usuario del contacto
    /// </summary>
    public Optional<ulong> ContactId { get; set; } = Optional<ulong>.None();
}
