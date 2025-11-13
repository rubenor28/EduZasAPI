namespace MinimalAPI.Application.DTOs.ClassStudents;

public sealed record ClassStudentUpdateMAPI
{
    public required ulong UserId { get; set; }
    public required string ClassId { get; set; }
    public required bool Hidden { get; set; }
}
