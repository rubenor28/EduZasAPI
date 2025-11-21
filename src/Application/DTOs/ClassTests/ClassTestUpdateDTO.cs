using Application.DTOs.Common;

namespace Application.DTOs.ClassTests;

public sealed record ClassTestUpdateDTO
{
    public required ulong TestId { get; set; }
    public required string ClassId { get; set; }
    public required bool Visible { get; init; }
    public required Executor Executor { get; init; }
}
