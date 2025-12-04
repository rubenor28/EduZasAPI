using Application.DTOs.Common;

namespace Application.DTOs.Tags;

public sealed record TagCriteriaDTO : CriteriaDTO
{
    /// <summary>
    // Texto de la etiqueta
    /// </summary>
    public StringQueryDTO? Text { get; init; } 

    /// <summary>
    // ID de usuario del dueño de la agenda
    /// </summary>
    public ulong? AgendaOwnerId { get; init; } 

    /// <summary>
    /// ID de usuario del contacto
    /// </summary>
    public ulong? ContactId { get; init; } 
}
