using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Tests;

public sealed record DeleteTestDTO : IIdentifiable<ulong>
{
    public required ulong Id { get; set; }
    public required Executor Executor { get; set; }
}
