using Application.DTOs.Common;

namespace Application.DTOs.Resources;

public sealed record NewResourceDTO
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required ulong ProfessorId { get; set; }
    public required Executor Executor { get; set; }
}
