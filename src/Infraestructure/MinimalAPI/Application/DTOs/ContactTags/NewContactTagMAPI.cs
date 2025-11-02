namespace MinimalAPI.Application.DTOs.ContactTags;

public sealed record NewContactTagMAPI
{
    public required ulong TagId { get; set; }
    public required ulong AgendaContactId { get; set; }
}
