using Application.DTOs.Common;

namespace Application.DTOs.Contacts;

/// <summary>
/// Criterios de búsqueda para contactos.
/// </summary>
public sealed record ContactCriteriaDTO : CriteriaDTO
{
    /// <summary>Filtra por alias.</summary>
    public StringQueryDTO? Alias { get; init; }

    /// <summary>Filtra por ID del dueño de la agenda.</summary>
    public ulong? AgendaOwnerId { get; init; }

    /// <summary>Filtra por ID del usuario contacto.</summary>
    public ulong? UserId { get; init; }

    /// <summary>Filtra por aquellos contactos que no sean profesores en una clase.</summary>
    public string? NotProfessorInClass { get; init; }

    /// <summary>Filtra por etiquetas.</summary>
    public IEnumerable<string>? Tags { get; init; }
}
