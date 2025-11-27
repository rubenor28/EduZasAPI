using Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.ClassResources;

public sealed record ClassResourceCriteriaDTO : CriteriaDTO
{
    public StringQueryMAPI? ClassId { get; init; }
    public Guid? ResourceId { get; init; }
    public bool? Hidden { get; init; }
}
