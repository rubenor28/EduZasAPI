using Application.DTOs.Common;

namespace Application.DTOs.ClassStudents;

public sealed record ToggleClassVisibilityDTO
{
    public required string ClassId { get; set; }
    public required Executor Executor { get; set; }
}
