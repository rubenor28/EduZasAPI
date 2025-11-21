using Application.DTOs.Common;
using Domain.Entities;

namespace Application.DTOs.ClassTests;

public sealed record DeleteClassTestDTO
{
    public required ClassTestIdDTO Id { get; set; }
    public required Executor Executor { get; set; }
}
