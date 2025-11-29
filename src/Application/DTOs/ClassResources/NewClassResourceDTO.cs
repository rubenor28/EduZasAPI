using Application.DTOs.Common;

namespace Application.DTOs.ClassResources;

public sealed class NewClassResourceDTO
{
    public required string ClassId { get; init; }
    public required Guid ResourceId { get; init; }
    public required bool Hidden { get; init; }
    public required Executor Executor { get; init; }
}
