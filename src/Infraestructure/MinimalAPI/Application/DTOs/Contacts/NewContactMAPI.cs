namespace MinimalAPI.Application.DTOs.Contacts;

public sealed record NewContactMAPI(
    string Alias,
    string? Notes,
    ulong AgendaOwnerId,
    ulong ContactId,
    IEnumerable<string>? Tags
);
