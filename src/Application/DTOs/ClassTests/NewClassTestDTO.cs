using Application.DTOs.Common;

namespace Application.DTOs.ClassTests;

public sealed record NewClassTestDTO
{
    public required ulong TestId { get; init; }
    public required string ClassId { get; init; }
    public required bool Visible { get; init; }
    public required Executor Executor { get; init; }
}
