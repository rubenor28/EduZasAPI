using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Resources;

public sealed record DeleteResourceDTO : IIdentifiable<ulong>
{
    public required ulong Id { get; set; }
    public required Executor Executor { get; set; }
}
