using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Users;

public sealed record ReadUserDTO : IIdentifiable<ulong>
{
    public required ulong Id { get; set; }
    public required Executor Executor { get; set; }
}
