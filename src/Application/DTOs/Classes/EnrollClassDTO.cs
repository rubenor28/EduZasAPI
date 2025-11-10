using Application.DTOs.Common;

namespace Application.DTOs.Classes;

public sealed record EnrollClassDTO
{
    public required string ClassId { get; set; }
    public required ulong UserId { get; set; }
    public required Executor Executor { get; set; }
}
