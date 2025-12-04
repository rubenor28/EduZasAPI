namespace Application.DTOs.ClassStudents;

public sealed record ClassStudentUpdateDTO
{
    public required ulong UserId { get; init; }
    public required string ClassId { get; init; }
    public required bool Hidden { get; init; }
}
