namespace MinimalAPI.Application.DTOs.Contacts;

public sealed record NewContactMAPI(
    string Alias,
    string? Notes,
    ulong AgendaOwnerId,
    ulong UserId,
    IEnumerable<string>? Tags
);
