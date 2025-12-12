namespace Application.DTOs.Resources;

public sealed record ReadResourceDTO(ulong UserId, string ClassId, Guid ResourceId);
