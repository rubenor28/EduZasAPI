using Application.DTOs.Common;

namespace Application.DTOs.Users;

public sealed record ReadUserDTO
{
    public required ulong Id { get; set; }
    public required Executor Executor { get; set; }
}
