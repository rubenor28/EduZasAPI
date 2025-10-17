namespace MinimalAPI.Application.DTOs.Classes;

public sealed record StudentClassRelationCriteriaMAPI
{
    public ulong? StudentId { get; set; }
    public string? ClassId { get; set; }
}
