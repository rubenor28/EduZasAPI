using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Resources;

public sealed record ResourceUpdateDTO : IIdentifiable<ulong>
{
    public required ulong Id { get; set; }
    public required bool Active { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required Executor Executor { get; set; }
}
