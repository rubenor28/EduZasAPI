using Application.DTOs.Common;

namespace Application.DTOs.ClassStudents;

public sealed record ClassStudentUpdateDTO
{
    public required ulong UserId { get; set; }
    public required string ClassId { get; set; }
    public required bool Hidden { get; set; }
    public required Executor Executor { get; set; }
}
