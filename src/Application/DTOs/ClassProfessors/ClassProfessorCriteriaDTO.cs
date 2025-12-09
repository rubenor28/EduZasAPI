using Application.DTOs.Common;

namespace Application.DTOs.ClassProfessors;

/// <summary>
/// Criterios de búsqueda para relaciones profesor-clase.
/// </summary>
public sealed record ClassProfessorCriteriaDTO : CriteriaDTO
{
    /// <summary>Filtra por ID de usuario.</summary>
    public ulong? UserId { get; init; } 

    /// <summary>Filtra por ID de clase.</summary>
    public string? ClassId { get; init; } 

    /// <summary>Filtra por estado de propietario.</summary>
    public bool? IsOwner { get; init; } 
}
