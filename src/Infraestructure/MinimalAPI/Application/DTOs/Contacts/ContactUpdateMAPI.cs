namespace MinimalAPI.Application.DTOs.Contacts;

public sealed class ContactUpdateMAPI
{
    public required ulong AgendaOwnerId { get; set; }
    public required ulong UserId { get; set; }
    public required string Alias { get; set; }
    public string? Notes { get; set; }
}
