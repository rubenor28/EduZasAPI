namespace MinimalAPI.Application.DTOs.ClassStudents;

public sealed record StudentClassRelationMAPI
{
    public required ulong StudentId { get; set; }
    public required string ClassId { get; set; }
    public required bool Hidden { get; set; }
}
