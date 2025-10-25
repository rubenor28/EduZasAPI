namespace Application.DTOs.ContactTag;

public sealed record NewContactTagDTO
{
    public required ulong TagId { get; set; }
    public required ulong AgendaContactId { get; set; }
}
