using Application.DTOs.Common;

namespace Application.DTOs.ClassResources;

public sealed class ClassResourceUpdateDTO
{
    public required string ClassId { get; init; }
    public required Guid ResourceId { get; init; }
    public bool Hidden { get; init; }
    public required Executor Executor { get; init; }
}
