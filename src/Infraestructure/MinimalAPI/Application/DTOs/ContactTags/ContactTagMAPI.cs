namespace MinimalAPI.Application.DTOs.ContactTags;

public sealed record ContactTagMAPI(string Tag, ulong AgendaOwnerId, ulong UserId);
