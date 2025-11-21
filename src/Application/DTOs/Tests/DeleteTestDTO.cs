using Application.DTOs.Common;

namespace Application.DTOs.Tests;

public sealed record DeleteTestDTO
{
    public required ulong Id { get; set; }
    public required Executor Executor { get; set; }
}
