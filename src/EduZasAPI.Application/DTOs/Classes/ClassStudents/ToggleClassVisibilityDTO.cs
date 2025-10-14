using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

public record class ToggleClassVisibilityDTO
{
    public required string ClassId { get; set; }
    public required Executor Executor { get; set; }
}
