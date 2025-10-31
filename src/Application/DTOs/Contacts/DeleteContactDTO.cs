using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Contacts;

public sealed record DeleteContactDTO : IIdentifiable<ulong>
{
    public required ulong Id { get; init; }
    public required Executor Executor { get; init; }
}
