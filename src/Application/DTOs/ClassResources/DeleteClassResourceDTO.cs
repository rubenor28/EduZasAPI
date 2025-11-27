using Application.DTOs.Common;

namespace Application.DTOs.ClassResources;

public sealed class DeleteClassResourceDTO
{
    public required string ClassId { get; init; }
    public required Guid ResourceId { get; init; }
    public required Executor Executor { get; init; }
}
