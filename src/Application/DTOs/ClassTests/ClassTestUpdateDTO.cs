using Application.DTOs.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.DTOs.ClassTests;

public sealed record ClassTestUpdateDTO : IIdentifiable<ClassTestIdDTO>
{
    public required ClassTestIdDTO Id { get; init; }
    public required bool Visible { get; init; }
    public required Executor Executor { get; init; }
}
