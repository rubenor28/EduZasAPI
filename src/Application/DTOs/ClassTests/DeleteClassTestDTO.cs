using Application.DTOs.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.DTOs.ClassTests;

public sealed record DeleteClassTestDTO : IIdentifiable<ClassTestIdDTO>
{
    public required ClassTestIdDTO Id { get; set; }
    public required Executor Executor { get; set; }
}
