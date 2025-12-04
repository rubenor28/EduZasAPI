using Application.DTOs.Common;

namespace Application.DTOs;

public sealed record UserActionDTO<T>
{
    public required T Data { get; init; }
    public required Executor Executor { get; init; }
}
