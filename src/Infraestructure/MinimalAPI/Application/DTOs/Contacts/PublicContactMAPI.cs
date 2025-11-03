namespace MinimalAPI.Application.DTOs.Contacts;

public sealed record PublicContactMAPI
{
    public required ulong AgendaOwnerId { get; init; }
    public required ulong ContactId { get; init; }
    public required string Alias { get; init; }
    public required string? Notes { get; init; }
}
