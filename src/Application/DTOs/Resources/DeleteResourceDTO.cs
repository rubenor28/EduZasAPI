using Application.DTOs.Common;

namespace Application.DTOs.Resources;

public sealed record DeleteResourceDTO
{
    public required Guid Id { get; set; }
    public required Executor Executor { get; set; }
}
