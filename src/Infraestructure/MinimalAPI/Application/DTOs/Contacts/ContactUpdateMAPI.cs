namespace MinimalAPI.Application.DTOs.Contacts;

public sealed class ContactUpdateMAPI
{
    public required ulong Id { get; set; }
    public required string Alias { get; set; }
    public string? Notes { get; set; }
    public required ulong AgendaOwnerId { get; set; }
    public required ulong ContactId { get; set; }
}
