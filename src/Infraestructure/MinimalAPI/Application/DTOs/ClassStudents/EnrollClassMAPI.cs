namespace MinimalAPI.Application.DTOs.ClassStudents;

public sealed record EnrollClassMAPI
{
    public required string ClassId { get; set; }
    public required ulong UserId { get; set; }
}
