using Domain.ValueObjects;

namespace Application.DTOs.ClassContent;

public sealed record ClassContentCriteriaDTO : CriteriaDTO
{
    public required string ClassId { get; set; }
    public StringQueryDTO? Title { get; set; }
    public bool? Visible { get; set; }
    public ContentType? Type { get; set; }
}
