namespace MinimalAPI.Application.DTOs.ClassStudents;

public sealed record StudentClassRelationCriteriaMAPI
{
    public ulong? StudentId { get; set; }
    public string? ClassId { get; set; }
}
